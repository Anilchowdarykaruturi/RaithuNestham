using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Models;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GovernmentSchemesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GovernmentSchemesController(
        ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetGovernmentSchemes()
    {
        var schemes =
            await _context.GovernmentSchemes
                .ToListAsync();

        return Ok(schemes);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetGovernmentScheme(int id)
    {
        var scheme =
            await _context.GovernmentSchemes
                .FindAsync(id);

        if (scheme == null)
        {
            return NotFound();
        }

        return Ok(scheme);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGovernmentScheme(
        GovernmentScheme scheme)
    {
        _context.GovernmentSchemes.Add(scheme);

        await _context.SaveChangesAsync();

        return Ok(scheme);
    }
}