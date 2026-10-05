using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Models;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FertilizersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FertilizersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetFertilizers()
    {
        var fertilizers =
            await _context.Fertilizers
                .ToListAsync();

        return Ok(fertilizers);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetFertilizer(int id)
    {
        var fertilizer =
            await _context.Fertilizers
                .FindAsync(id);

        if (fertilizer == null)
        {
            return NotFound();
        }

        return Ok(fertilizer);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFertilizer(
        Fertilizer fertilizer)
    {
        _context.Fertilizers.Add(fertilizer);

        await _context.SaveChangesAsync();

        return Ok(fertilizer);
    }
}