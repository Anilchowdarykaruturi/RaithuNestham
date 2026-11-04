
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Services.Interfaces;
using RaithuNestham.Services.RAG;

namespace RaithuNestham.Services;

public class AIService : IAIService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly RagRetrievalService _ragRetrievalService;
    private readonly WeatherForecastService _weatherForecastService;

    public AIService(
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        Microsoft.Extensions.Configuration.IConfiguration configuration,
        RagRetrievalService ragRetrievalService,
        WeatherForecastService weatherForecastService)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
        _ragRetrievalService = ragRetrievalService;
        _weatherForecastService = weatherForecastService;
    }

    public async Task<string> AskAsync(
        int farmerId,
        string question)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            return "Please enter a question.";
        }

        var farmer = await _context.Farmers
            .Include(x => x.Fields)
            .FirstOrDefaultAsync(x => x.Id == farmerId);

        if (farmer == null)
        {
            return "Farmer information was not found.";
        }

        var apiKey =
            Environment.GetEnvironmentVariable("GEMINI_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "GEMINI_API_KEY environment variable is not configured.");
        }

        // STEP 1: Retrieve relevant farming knowledge using RAG.
        var relevantChunks =
            await _ragRetrievalService.RetrieveRelevantChunksAsync(
                question,
                3);

        var retrievedContext = relevantChunks.Count > 0
            ? string.Join(
                "\n\n--- Retrieved document section ---\n\n",
                relevantChunks)
            : "No relevant information was found in the farming knowledge documents.";

        // STEP 2: Retrieve actual weather data when relevant.
        var weatherContext =
            await GetWeatherContextAsync(question, farmer);

        // STEP 3: Build the bilingual AI prompt.
        var prompt =
            "You are RaithuNestham, an AI farming assistant for farmers in Andhra Pradesh. " +
            "Give clear, practical, safe and easy-to-understand farming guidance. " +
            "Consider the farmer's information when relevant.\n\n" +

            "RETRIEVED FARMING KNOWLEDGE:\n" +
            retrievedContext + "\n\n" +

            "WEATHER DATA:\n" +
            weatherContext + "\n\n" +

            "RAG GROUNDING RULES:\n" +
            "Use retrieved farming knowledge when relevant. Do not claim that documents " +
            "contain information they do not contain. If information is insufficient, " +
            "say so. Distinguish general knowledge from retrieved information. " +
            "Never invent facts or claim to have consulted a source when you have not.\n\n" +

            "WEATHER GROUNDING RULES:\n" +
            "Use supplied weather data only when it contains actual observations or forecasts. " +
            "Never invent temperature, rainfall, humidity, wind speed, or forecast values. " +
            "Clearly distinguish current observations, historical observations and future forecasts. " +
            "Use the requested location, not automatically the farmer's registered village, " +
            "when the question names another location. If weather data is unavailable, say so. " +
            "Never present a forecast as a certainty.\n\n" +

            "FARMING SAFETY RULES:\n" +
            "Consider crop, crop stage, soil, irrigation, season, and local agricultural " +
            "recommendations where relevant. Do not invent pesticide or fertilizer dosages, " +
            "product labels, prices, government schemes, or official recommendations. " +
            "Advise farmers to follow product labels and local agricultural guidance. " +
            "Ask a short follow-up question when important information is missing.\n\n" +

            "LANGUAGE RULE - MANDATORY:\n" +
            "Answer every question in BOTH Telugu and English. " +
            "Write the Telugu section in Telugu script, never Telugu transliteration. " +
            "Always provide Telugu first and English second.\n\n" +

            "REQUIRED RESPONSE FORMAT:\n\n" +
            "తెలుగు:\n" +
            "[Complete answer in Telugu script]\n\n" +
            "English:\n" +
            "[Complete English translation]\n\n" +

            $"Farmer name: {farmer.FullName}\n" +
            $"Registered village: {farmer.Village}\n" +
            $"Registered mandal: {farmer.Mandal}\n" +
            $"Registered district: {farmer.District}\n" +
            $"Question: {question}";

        // STEP 4: Send the prompt to Gemini.
        var requestBody = new
        {
            model = "gemini-3.6-flash",
            input = prompt
        };

        var json = JsonSerializer.Serialize(requestBody);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://generativelanguage.googleapis.com/v1beta/interactions");

        request.Headers.Add("x-goog-api-key", apiKey);

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        using var response =
            await _httpClient.SendAsync(request);

        var responseContent =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            // Do not expose the API key or provider response to the client.
            throw new InvalidOperationException(
                $"Gemini API request failed with HTTP status {(int)response.StatusCode}.");
        }

        using var document =
            JsonDocument.Parse(responseContent);

        if (document.RootElement.TryGetProperty(
                "output",
                out var output))
        {
            if (output.ValueKind == JsonValueKind.String)
            {
                return output.GetString()
                    ?? "No response received from Gemini.";
            }

            // Some API responses may represent output as structured JSON.
            return output.ToString();
        }

        if (document.RootElement.TryGetProperty(
                "steps",
                out var steps))
        {
            foreach (var step in steps.EnumerateArray())
            {
                if (step.TryGetProperty(
                        "type",
                        out var type) &&
                    type.GetString() == "model_output")
                {
                    if (step.TryGetProperty(
                            "content",
                            out var content) &&
                        content.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in content.EnumerateArray())
                        {
                            if (item.TryGetProperty(
                                    "text",
                                    out var text))
                            {
                                return text.GetString()
                                    ?? "No response received from Gemini.";
                            }
                        }
                    }
                }
            }
        }

        return "No response received from Gemini.";
    }

    // Retrieves weather data for the location requested by the user,
    // or the farmer's registered village when no location is specified.
    private async Task<string> GetWeatherContextAsync(
        string question,
        RaithuNestham.Models.Farmer farmer)
    {
        var weatherKeywords = new[]
        {
            "weather", "rain", "rainfall", "temperature",
            "forecast", "humidity", "wind",
            "വాతావరణం", "వాతావరణ", "వర్షం",
            "ఉష్ణోగ్రత", "వర్షపాతం", "గాలి",
            "నిన్న", "రేపు", "ninna", "repu"
        };

        var asksAboutWeather = weatherKeywords.Any(keyword =>
            question.Contains(
                keyword,
                StringComparison.OrdinalIgnoreCase));

        if (!asksAboutWeather)
        {
            return "No weather lookup was requested. Do not invent weather conditions.";
        }

        // Extract common English location phrases.
        string? requestedLocation = null;

        var locationMatch = Regex.Match(
            question,
            @"\b(?:weather|rain|rainfall|temperature|forecast|humidity|wind)\s+(?:in|at|for)\s+([A-Za-z][A-Za-z\s-]{1,50}?)(?=\s+(?:today|tomorrow|yesterday|right now|currently|next week|this week)\b|[?.!,;]|$)",
            RegexOptions.IgnoreCase);

        if (locationMatch.Success)
        {
            requestedLocation = locationMatch.Groups[1].Value.Trim();
        }

        var village = farmer.Village?.Trim();
        var mandal = farmer.Mandal?.Trim();
        var district = farmer.District?.Trim();
        var state = farmer.State?.Trim();

        string locationQuery;

        if (!string.IsNullOrWhiteSpace(requestedLocation))
        {
            // An explicitly requested location takes precedence.
            locationQuery = $"{requestedLocation}, India";
        }
        else
        {
            if (string.IsNullOrWhiteSpace(village))
            {
                return "The farmer's registered village is missing. " +
                       "Ask the farmer to update their profile. " +
                       "Do not invent weather data.";
            }

            locationQuery = string.Join(
                ", ",
                new[] { village, mandal, district, state, "India" }
                    .Where(part => !string.IsNullOrWhiteSpace(part))
                    .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        var lowerQuestion = question.ToLowerInvariant();

        // Interpret common relative dates using India local time.
        var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows()
                ? "India Standard Time"
                : "Asia/Kolkata");

        var indiaToday = DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, indiaTimeZone)
                .DateTime);

        DateOnly? historicalDate = null;

        if (lowerQuestion.Contains("day before yesterday"))
        {
            historicalDate = indiaToday.AddDays(-2);
        }
        else if (lowerQuestion.Contains("yesterday") ||
                 lowerQuestion.Contains("ninna") ||
                 question.Contains("నిన్న"))
        {
            historicalDate = indiaToday.AddDays(-1);
        }

        var asksTomorrow =
            lowerQuestion.Contains("tomorrow") ||
            lowerQuestion.Contains("repu") ||
            question.Contains("రేపు");

        try
        {
            using var search =
                await _weatherForecastService.SearchLocationAsync(
                    locationQuery);

            if (!search.RootElement.TryGetProperty(
                    "results",
                    out var matches) ||
                matches.ValueKind != JsonValueKind.Array ||
                matches.GetArrayLength() == 0)
            {
                return $"The weather location '{locationQuery}' could not be found. " +
                       "Ask the farmer to verify the location. " +
                       "Do not substitute another village or invent weather data.";
            }

            // Prefer a result whose country is India.
            JsonElement match = default;

            foreach (var candidate in matches.EnumerateArray())
            {
                if (candidate.TryGetProperty("country", out var country) &&
                    string.Equals(
                        country.GetString(),
                        "India",
                        StringComparison.OrdinalIgnoreCase))
                {
                    match = candidate;
                    break;
                }
            }

            if (match.ValueKind != JsonValueKind.Object)
            {
                match = matches[0];
            }

            var latitude = match.GetProperty("latitude").GetDouble();
            var longitude = match.GetProperty("longitude").GetDouble();

            var resolvedName =
                match.TryGetProperty("name", out var nameElement)
                    ? nameElement.GetString() ?? locationQuery
                    : locationQuery;

            var locationContext =
                $"Requested location: {locationQuery}\n" +
                $"Provider location match: {resolvedName}\n" +
                $"Coordinates: {latitude}, {longitude}\n" +
                "Weather provider: Open-Meteo\n";

            // Historical weather.
            if (historicalDate.HasValue)
            {
                using var historical =
                    await _weatherForecastService.GetHistoricalWeatherAsync(
                        latitude,
                        longitude,
                        historicalDate.Value);

                return locationContext +
                       $"Historical date requested: {historicalDate.Value:yyyy-MM-dd}\n" +
                       "Historical daily weather response:\n" +
                       historical.RootElement.GetRawText() + "\n\n" +
                       "Use only values in this response. If daily data is absent, " +
                       "explain that historical data is unavailable for this date.";
            }

            // Forecast questions about tomorrow.
            if (asksTomorrow)
            {
                using var forecast =
                    await _weatherForecastService.GetForecastAsync(
                        latitude,
                        longitude);

                return locationContext +
                       "Seven-day forecast response:\n" +
                       forecast.RootElement.GetRawText() + "\n\n" +
                       "Answer specifically for tomorrow using the date in the response. " +
                       "Do not invent missing values.";
            }

            // Current conditions and the seven-day forecast.
            using var current =
                await _weatherForecastService.GetCurrentWeatherAsync(
                    latitude,
                    longitude);

            using var sevenDayForecast =
                await _weatherForecastService.GetForecastAsync(
                    latitude,
                    longitude);

            return locationContext +
                   "Current weather response:\n" +
                   current.RootElement.GetRawText() + "\n\n" +
                   "Seven-day forecast response:\n" +
                   sevenDayForecast.RootElement.GetRawText() + "\n\n" +
                   "Use only values present in these responses. " +
                   "Clearly distinguish current conditions from forecasts. " +
                   "If the location match appears inconsistent, explain that " +
                   "the location needs verification.";
        }
        catch (Exception ex) when (
            ex is HttpRequestException ||
            ex is TaskCanceledException ||
            ex is JsonException ||
            ex is ArgumentException ||
            ex is TimeZoneNotFoundException ||
            ex is InvalidTimeZoneException)
        {
            return $"Weather data could not be retrieved for {locationQuery}. " +
                   "Explain that the weather service is temporarily unavailable. " +
                   "Do not invent weather values.";
        }
    }
}
