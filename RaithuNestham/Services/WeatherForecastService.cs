
using System.Text.Json;

namespace RaithuNestham.Services;

public class WeatherForecastService
{
    private readonly HttpClient _httpClient;

    public WeatherForecastService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

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
            "&timezone=Asia%2FKolkata";

        using var response = await _httpClient.GetAsync(
            url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(
            cancellationToken);

        return JsonDocument.Parse(json);
    }

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
            "&timezone=Asia%2FKolkata" +
            "&forecast_days=7";

        using var response = await _httpClient.GetAsync(
            url, cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(
            cancellationToken);

        return JsonDocument.Parse(json);
    }
}
