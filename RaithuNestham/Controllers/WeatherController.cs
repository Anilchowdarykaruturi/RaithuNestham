using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Models;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WeatherController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public WeatherController(
        ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetWeather(
        string? village)
    {
        var query = _context.WeatherLogs
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(village))
        {
            query = query.Where(
                x => x.Village == village);
        }

        var weather = await query
            .OrderByDescending(x => x.RecordedAt)
            .Take(10)
            .ToListAsync();

        return Ok(weather);
    }

    [HttpPost]
    public async Task<IActionResult> AddWeather(
        WeatherLog weather)
    {
        weather.RecordedAt = DateTime.UtcNow;

        _context.WeatherLogs.Add(weather);

        await _context.SaveChangesAsync();

        return Ok(weather);
    }
}
