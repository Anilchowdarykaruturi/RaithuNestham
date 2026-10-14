
using System.Net;
using System.Text;
using System.Text.Json;

namespace RaithuNestham.Services.RAG;

public class EmbeddingService
{
    private readonly HttpClient _httpClient;

    public EmbeddingService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<List<double>> CreateEmbeddingAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException(
                "Text cannot be empty.", nameof(text));
        }

        var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "GEMINI_API_KEY is not configured.");
        }

        var requestBody = new
        {
            content = new
            {
                parts = new[]
                {
                    new { text }
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);

        const string url =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-embedding-001:embedContent";

        const int maxAttempts = 4;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post, url);

            request.Headers.Add("x-goog-api-key", apiKey);

            request.Content = new StringContent(
                json, Encoding.UTF8, "application/json");

            using var response =
                await _httpClient.SendAsync(request);

            var responseContent =
                await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                using var document =
                    JsonDocument.Parse(responseContent);

                var values = document.RootElement
                    .GetProperty("embedding")
                    .GetProperty("values");

                return values
                    .EnumerateArray()
                    .Select(value => value.GetDouble())
                    .ToList();
            }

            var isTransient =
                response.StatusCode == HttpStatusCode.TooManyRequests ||
                response.StatusCode == HttpStatusCode.InternalServerError ||
                response.StatusCode == HttpStatusCode.BadGateway ||
                response.StatusCode == HttpStatusCode.ServiceUnavailable ||
                response.StatusCode == HttpStatusCode.GatewayTimeout;

            if (!isTransient || attempt == maxAttempts)
            {
                throw new InvalidOperationException(
                    $"Gemini embedding API error: {response.StatusCode} - {responseContent}");
            }

            var delaySeconds = Math.Pow(2, attempt);

            Console.WriteLine(
                $"Gemini embedding API returned {(int)response.StatusCode}. " +
                $"Retrying attempt {attempt + 1} of {maxAttempts} " +
                $"after {delaySeconds} seconds.");

            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }

        throw new InvalidOperationException(
            "Gemini embedding request failed after all retry attempts.");
    }
}
