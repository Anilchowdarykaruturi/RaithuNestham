
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Models;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeviceRegistrationsController : ControllerBase
{
	private readonly ApplicationDbContext _context;

	public DeviceRegistrationsController(
		ApplicationDbContext context)
	{
		_context = context;
	}

	[HttpPost("register")]
	public async Task<IActionResult> RegisterDevice(
		[FromBody] RegisterDeviceRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.FcmToken))
		{
			return BadRequest(new
			{
				message = "FCM token is required."
			});
		}

		if (request.FcmToken.Length > 2048)
		{
			return BadRequest(new
			{
				message = "FCM token is too long."
			});
		}

		var userIdValue = User.FindFirstValue(
			ClaimTypes.NameIdentifier);

		if (!int.TryParse(userIdValue, out var userId))
		{
			return Unauthorized(new
			{
				message = "Invalid user identity."
			});
		}

		var token = request.FcmToken.Trim();

		var existingDevice = await _context.DeviceRegistrations
			.FirstOrDefaultAsync(x => x.FcmToken == token);

		if (existingDevice != null)
		{
			existingDevice.UserId = userId;
			existingDevice.Platform = "Android";
			existingDevice.IsActive = true;
			existingDevice.UpdatedAt = DateTime.UtcNow;
		}
		else
		{
			var device = new DeviceRegistration
			{
				UserId = userId,
				FcmToken = token,
				Platform = "Android",
				IsActive = true,
				CreatedAt = DateTime.UtcNow,
				UpdatedAt = DateTime.UtcNow
			};

			_context.DeviceRegistrations.Add(device);
		}

		await _context.SaveChangesAsync();

		return Ok(new
		{
			success = true,
			message = "Device registered successfully."
		});
	}
}

public class RegisterDeviceRequest
{
	public string FcmToken { get; set; } = string.Empty;
}
