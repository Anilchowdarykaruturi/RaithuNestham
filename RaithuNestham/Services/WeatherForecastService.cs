
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RaithuNestham.Services;

public class WeatherForecastService
{
    private readonly HttpClient _httpClient;

    public WeatherForecastService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(20);
    }

    // Search for a village, town, or city worldwide.
    public async Task<JsonDocument> SearchLocationAsync(
        string name,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            name.Trim().Length < 2)
        {
            throw new ArgumentException(
                "Enter at least two characters for the location.",
                nameof(name));
        }

        var url =
            "https://geocoding-api.open-meteo.com/v1/search" +
            $"?name={Uri.EscapeDataString(name.Trim())}" +
            "&count=5&language=en&format=json";

        using var response =
            await _httpClient.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        return JsonDocument.Parse(json);
    }

    // Get current weather for coordinates.
    public async Task<JsonDocument> GetCurrentWeatherAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        var url =
            "https://api.open-meteo.com/v1/forecast" +
            $"?latitude={latitude}" +
            $"&longitude={longitude}" +
            "&current=temperature_2m,relative_humidity_2m," +
            "precipitation,rain,showers,weather_code" +
            "&timezone=auto";

        using var response =
            await _httpClient.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        return JsonDocument.Parse(json);
    }

    // Get a seven-day forecast for coordinates.
    public async Task<JsonDocument> GetForecastAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        var url =
            "https://api.open-meteo.com/v1/forecast" +
            $"?latitude={latitude}" +
            $"&longitude={longitude}" +
            "&daily=temperature_2m_max,temperature_2m_min," +
            "precipitation_probability_max,precipitation_sum," +
            "wind_speed_10m_max" +
            "&timezone=auto&forecast_days=7";

        using var response =
            await _httpClient.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        return JsonDocument.Parse(json);
    }

    // Get historical daily weather for a specific date.
    public async Task<JsonDocument> GetHistoricalWeatherAsync(
        double latitude,
        double longitude,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var requestedDate = date.ToString("yyyy-MM-dd");

        var url =
            "https://archive-api.open-meteo.com/v1/archive" +
            $"?latitude={latitude}" +
            $"&longitude={longitude}" +
            $"&start_date={requestedDate}" +
            $"&end_date={requestedDate}" +
            "&daily=temperature_2m_max,temperature_2m_min," +
            "precipitation_sum,rain_sum,wind_speed_10m_max" +
            "&timezone=auto";

        using var response =
            await _httpClient.GetAsync(url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var json =
            await response.Content.ReadAsStringAsync(
                cancellationToken);

        return JsonDocument.Parse(json);
    }
}
