
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Models;
using RaithuNestham.Services;

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

    // GET: /api/Weather
    // GET: /api/Weather?village=Kaldhari
    // Searches for any village or town supported by Open-Meteo geocoding.
    [HttpGet]
    public async Task<IActionResult> GetWeather(
        [FromQuery] string? village = null,
        CancellationToken cancellationToken = default)
    {
        var locations =
            new List<(string Village, double Latitude, double Longitude)>
            {
                ("Kaldhari", 16.977152, 81.80249),
                ("Rajahmundry", 17.0005, 81.8040),
                ("Amalapuram", 16.5787, 82.0061)
            };

        if (!string.IsNullOrWhiteSpace(village))
        {
            village = village.Trim();

            if (village.Length < 2)
            {
                return BadRequest(new
                {
                    message = "Enter at least two characters for a village or town."
                });
            }

            try
            {
                using var search =
                    await _forecastService.SearchLocationAsync(
                        village,
                        cancellationToken);

                if (!search.RootElement.TryGetProperty(
                        "results", out var matches) ||
                    matches.ValueKind != JsonValueKind.Array ||
                    matches.GetArrayLength() == 0)
                {
                    return NotFound(new
                    {
                        message = $"No location found for '{village}'. Try another spelling or include the district or country."
                    });
                }

                var match = matches[0];

                var name = match.GetProperty("name").GetString()
                    ?? village;

                var admin1 = match.TryGetProperty("admin1", out var a)
                    ? a.GetString()
                    : null;

                var country = match.TryGetProperty("country", out var c)
                    ? c.GetString()
                    : null;

                var displayName = string.Join(
                    ", ",
                    new[] { name, admin1, country }
                        .Where(part => !string.IsNullOrWhiteSpace(part))
                        .Distinct(StringComparer.OrdinalIgnoreCase));

                var latitude = match.GetProperty("latitude").GetDouble();
                var longitude = match.GetProperty("longitude").GetDouble();

                locations.Clear();
                locations.Add((displayName, latitude, longitude));
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new
                {
                    message = "Location search is temporarily unavailable."
                });
            }
            catch (TaskCanceledException)
                when (!cancellationToken.IsCancellationRequested)
            {
                return StatusCode(504, new
                {
                    message = "Location search timed out. Please try again."
                });
            }
            catch (JsonException)
            {
                return StatusCode(502, new
                {
                    message = "The location service returned invalid data."
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

                if (!root.TryGetProperty("current", out var current))
                {
                    return StatusCode(502, new
                    {
                        message = $"Current weather is unavailable for {location.Village}."
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
                    latitude = location.Latitude,
                    longitude = location.Longitude,
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
                    message = $"Weather is temporarily unavailable for {location.Village}."
                });
            }
            catch (TaskCanceledException)
                when (!cancellationToken.IsCancellationRequested)
            {
                return StatusCode(504, new
                {
                    message = $"The weather request timed out for {location.Village}."
                });
            }
            catch (JsonException)
            {
                return StatusCode(502, new
                {
                    message = $"Invalid weather data was received for {location.Village}."
                });
            }
            catch (KeyNotFoundException)
            {
                return StatusCode(502, new
                {
                    message = $"Required weather information is missing for {location.Village}."
                });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(502, new
                {
                    message = $"Weather information could not be read for {location.Village}."
                });
            }
        }

        return Ok(results);
    }

    // GET: /api/Weather/forecast?latitude=16.977152&longitude=81.80249
    [HttpGet("forecast")]
    public async Task<IActionResult> GetForecast(
        [FromQuery] double latitude = 16.977152,
        [FromQuery] double longitude = 81.80249,
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
                message = "Weather forecast is temporarily unavailable."
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
}
