using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipmentSubsidiesController : ControllerBase
{
	private readonly ApplicationDbContext _context;

	public EquipmentSubsidiesController(ApplicationDbContext context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<IActionResult> GetEquipmentSubsidies()
	{
		var subsidies = await _context.EquipmentSubsidies
			.AsNoTracking()
			.ToListAsync();

		return Ok(subsidies);
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetEquipmentSubsidy(int id)
	{
		var subsidy = await _context.EquipmentSubsidies
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Id == id);

		if (subsidy == null)
		{
			return NotFound(new
			{
				message = "Equipment subsidy not found."
			});
		}

		return Ok(subsidy);
	}
}