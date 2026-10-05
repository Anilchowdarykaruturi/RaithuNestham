using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Models;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FieldsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FieldsController(
        ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("farmer/{farmerId:int}")]
    public async Task<IActionResult> GetFields(
        int farmerId)
    {
        var fields = await _context.Fields
            .Where(x => x.FarmerId == farmerId)
            .ToListAsync();

        return Ok(fields);
    }

    [HttpPost]
    public async Task<IActionResult> CreateField(
        Field field)
    {
        _context.Fields.Add(field);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetFields),
            new
            {
                farmerId = field.FarmerId
            },
            field);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteField(
        int id)
    {
        var field =
            await _context.Fields.FindAsync(id);

        if (field == null)
        {
            return NotFound();
        }

        _context.Fields.Remove(field);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}