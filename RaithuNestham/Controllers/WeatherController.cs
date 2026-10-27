
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Models;
using RaithuNestham.Services;
using System.Text.Json;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WeatherController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly WeatherForecastService _forecastService;
    private readonly WeatherAdvisoryService _advisoryService;

    public WeatherController(
        ApplicationDbContext context,
        WeatherForecastService forecastService,
        WeatherAdvisoryService advisoryService)
    {
        _context = context;
        _forecastService = forecastService;
        _advisoryService = advisoryService;
    }

    // Get current weather for Kaldhari, Rajahmundry, and Amalapuram.
    [HttpGet]
    public async Task<IActionResult> GetWeather(
        [FromQuery] string? village = null,
        CancellationToken cancellationToken = default)
    {
        var locations = new[]
        {
            new
            {
                Village = "Kaldhari",
                Latitude = 16.977152,
                Longitude = 81.80249
            },
            new
            {
                Village = "Rajahmundry",
                Latitude = 17.0005,
                Longitude = 81.8040
            },
            new
            {
                Village = "Amalapuram",
                Latitude = 16.5787,
                Longitude = 82.0061
            }
        };

        if (!string.IsNullOrWhiteSpace(village))
        {
            locations = locations
                .Where(x => string.Equals(
                    x.Village,
                    village.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (locations.Length == 0)
            {
                return BadRequest(new
                {
                    message =
                        "Unknown village. Try Kaldhari, Rajahmundry, or Amalapuram."
                });
            }
        }

        var results = new List<object>();

        foreach (var location in locations)
        {
            try
            {
                using var weather =
                    await _forecastService.GetCurrentWeatherAsync(
                        location.Latitude,
                        location.Longitude,
                        cancellationToken);

                var root = weather.RootElement;

                if (!root.TryGetProperty(
                        "current",
                        out var current))
                {
                    return StatusCode(502, new
                    {
                        message =
                            $"Current weather data is missing for {location.Village}."
                    });
                }

                var temperature =
                    current.GetProperty("temperature_2m").GetDecimal();

                var humidity =
                    current.GetProperty("relative_humidity_2m").GetDecimal();

                var rainfall =
                    current.GetProperty("precipitation").GetDecimal();

                var weatherCode =
                    current.GetProperty("weather_code").GetInt32();

                var recordedAt =
                    current.GetProperty("time").GetString();

                var condition = weatherCode switch
                {
                    0 => "Clear sky",
                    1 => "Mainly clear",
                    2 => "Partly Cloudy",
                    3 => "Overcast",
                    45 or 48 => "Foggy",
                    51 or 53 or 55 => "Drizzle",
                    56 or 57 => "Freezing drizzle",
                    61 or 63 or 65 => "Rain",
                    66 or 67 => "Freezing rain",
                    71 or 73 or 75 => "Snow",
                    77 => "Snow grains",
                    80 or 81 or 82 => "Rain showers",
                    85 or 86 => "Snow showers",
                    95 => "Thunderstorm",
                    96 or 99 => "Thunderstorm with hail",
                    _ => "Unknown"
                };

                results.Add(new
                {
                    id = (int?)null,
                    village = location.Village,
                    temperature,
                    humidity,
                    rainfall,
                    weatherCondition = condition,
                    recordedAt,
                    source = "Open-Meteo"
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new
                {
                    message =
                        $"Weather is temporarily unavailable for {location.Village}. Please try again."
                });
            }
            catch (TaskCanceledException)
                when (!cancellationToken.IsCancellationRequested)
            {
                return StatusCode(504, new
                {
                    message =
                        $"The weather request timed out for {location.Village}."
                });
            }
            catch (JsonException)
            {
                return StatusCode(502, new
                {
                    message =
                        $"Invalid weather data was received for {location.Village}."
                });
            }
            catch (KeyNotFoundException)
            {
                return StatusCode(502, new
                {
                    message =
                        $"Required weather information is missing for {location.Village}."
                });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(502, new
                {
                    message =
                        $"Weather information could not be read for {location.Village}."
                });
            }
        }

        return Ok(results);
    }

    // Save a weather record manually.
    [HttpPost]
    public async Task<IActionResult> AddWeather(
        [FromBody] WeatherLog weather)
    {
        weather.RecordedAt = DateTime.UtcNow;

        _context.WeatherLogs.Add(weather);
        await _context.SaveChangesAsync();

        return Ok(weather);
    }

    // Get the seven-day weather forecast.
    [HttpGet("forecast")]
    public async Task<IActionResult> GetForecast(
        double latitude = 17.0,
        double longitude = 81.8,
        CancellationToken cancellationToken = default)
    {
        if (latitude < -90 || latitude > 90 ||
            longitude < -180 || longitude > 180)
        {
            return BadRequest(new
            {
                message = "Invalid latitude or longitude."
            });
        }

        try
        {
            using var forecast =
                await _forecastService.GetForecastAsync(
                    latitude,
                    longitude,
                    cancellationToken);

            return Ok(forecast.RootElement.Clone());
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, new
            {
                message =
                    "Weather forecast is temporarily unavailable."
            });
        }
        catch (TaskCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(504, new
            {
                message = "Weather forecast request timed out."
            });
        }
        catch (JsonException)
        {
            return StatusCode(502, new
            {
                message = "Weather forecast data was invalid."
            });
        }
    }

    // Get tomorrow's Telugu farming advice for any crop.
    [HttpGet("advice")]
    public async Task<IActionResult> GetWeatherAdvice(
        string cropName = "paddy",
        string growthStage = "growing",
        double latitude = 16.977152,
        double longitude = 81.80249,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cropName) ||
            string.IsNullOrWhiteSpace(growthStage))
        {
            return BadRequest(new
            {
                message =
                    "Crop name and growth stage are required."
            });
        }

        if (latitude < -90 || latitude > 90 ||
            longitude < -180 || longitude > 180)
        {
            return BadRequest(new
            {
                message = "Invalid latitude or longitude."
            });
        }

        try
        {
            using var forecast =
                await _forecastService.GetForecastAsync(
                    latitude,
                    longitude,
                    cancellationToken);

            var daily = forecast.RootElement.GetProperty("daily");
            var dates = daily.GetProperty("time");

            var rainProbabilities =
                daily.GetProperty(
                    "precipitation_probability_max");

            var rainfallAmounts =
                daily.GetProperty("precipitation_sum");

            if (dates.GetArrayLength() < 2 ||
                rainProbabilities.GetArrayLength() < 2 ||
                rainfallAmounts.GetArrayLength() < 2)
            {
                return StatusCode(503, new
                {
                    message =
                        "రేపటి వాతావరణ సమాచారం అందుబాటులో లేదు."
                });
            }

            var rainProbability =
                rainProbabilities[1].GetDouble();

            var rainfallMm =
                rainfallAmounts[1].GetDouble();

            var windSpeedKmh = 0.0;

            if (daily.TryGetProperty(
                    "wind_speed_10m_max",
                    out var windSpeeds) &&
                windSpeeds.ValueKind == JsonValueKind.Array &&
                windSpeeds.GetArrayLength() > 1)
            {
                windSpeedKmh = windSpeeds[1].GetDouble();
            }

            var advice = _advisoryService.GetCropAdvice(
                cropName,
                growthStage,
                rainProbability,
                rainfallMm,
                windSpeedKmh);

            return Ok(new
            {
                forecastDate = dates[1].GetString(),
                advice
            });
        }
        catch (HttpRequestException)
        {
            return StatusCode(503, new
            {
                message =
                    "వాతావరణ సమాచారం ప్రస్తుతం అందుబాటులో లేదు."
            });
        }
        catch (TaskCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(504, new
            {
                message =
                    "వాతావరణ సమాచారం పొందడానికి సమయం మించిపోయింది."
            });
        }
        catch (JsonException)
        {
            return StatusCode(502, new
            {
                message = "వాతావరణ సమాచారం సరిగ్గా అందలేదు."
            });
        }
        catch (KeyNotFoundException)
        {
            return StatusCode(502, new
            {
                message =
                    "వాతావరణ సమాచారం అవసరమైన వివరాలను కలిగి లేదు."
            });
        }
        catch (InvalidOperationException)
        {
            return StatusCode(502, new
            {
                message =
                    "వాతావరణ సమాచారం చదవడంలో సమస్య ఏర్పడింది."
            });
        }
    }
}
