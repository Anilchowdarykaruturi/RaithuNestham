using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Models;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CropsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CropsController(
        ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCrops()
    {
        var crops =
            await _context.Crops.ToListAsync();

        return Ok(crops);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCrop(int id)
    {
        var crop =
            await _context.Crops.FindAsync(id);

        if (crop == null)
        {
            return NotFound();
        }

        return Ok(crop);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCrop(
        Crop crop)
    {
        _context.Crops.Add(crop);

        await _context.SaveChangesAsync();

        return Ok(crop);
    }
}