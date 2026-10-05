using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FarmersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FarmersController(
        ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetFarmers()
    {
        var farmers = await _context.Farmers
            .Include(x => x.Fields)
            .ToListAsync();

        return Ok(farmers);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetFarmer(int id)
    {
        var farmer = await _context.Farmers
            .Include(x => x.Fields)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (farmer == null)
        {
            return NotFound(new
            {
                message = "Farmer not found."
            });
        }

        return Ok(farmer);
    }
}