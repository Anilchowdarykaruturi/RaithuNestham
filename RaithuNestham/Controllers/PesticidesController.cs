using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Models;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PesticidesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PesticidesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetPesticides()
    {
        var pesticides = await _context.Pesticides
            .ToListAsync();

        return Ok(pesticides);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPesticide(int id)
    {
        var pesticide = await _context.Pesticides
            .FindAsync(id);

        if (pesticide == null)
        {
            return NotFound();
        }

        return Ok(pesticide);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePesticide(
        Pesticide pesticide)
    {
        _context.Pesticides.Add(pesticide);

        await _context.SaveChangesAsync();

        return Ok(pesticide);
    }
}