
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Services;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
	private readonly ApplicationDbContext _context;
	private readonly FirebasePushNotificationService _pushService;

	public NotificationController(
		ApplicationDbContext context,
		FirebasePushNotificationService pushService)
	{
		_context = context;
		_pushService = pushService;
	}

	[HttpPost("test")]
	public async Task<IActionResult> SendTestNotification()
	{
		var userIdValue = User.FindFirstValue(
			ClaimTypes.NameIdentifier);

		if (!int.TryParse(userIdValue, out var userId))
		{
			return Unauthorized(new
			{
				message = "Invalid user identity."
			});
		}

		var devices = await _context.DeviceRegistrations
			.Where(device =>
				device.UserId == userId &&
				device.IsActive)
			.ToListAsync();

		if (devices.Count == 0)
		{
			return NotFound(new
			{
				message = "No registered Android devices found."
			});
		}

		var successfulNotifications = 0;

		foreach (var device in devices)
		{
			await _pushService.SendToDeviceAsync(
				device.FcmToken,
				"RaithuNestham Weather Alert",
				"Your weather notification system is ready!");

			successfulNotifications++;
		}

		return Ok(new
		{
			success = true,
			message = "Test notification sent successfully.",
			devicesNotified = successfulNotifications
		});
	}
}
