
namespace RaithuNestham.Models;

public class DeviceRegistration
{
	public int Id { get; set; }

	public int UserId { get; set; }

	public string FcmToken { get; set; } = string.Empty;

	public string Platform { get; set; } = "Android";

	public bool IsActive { get; set; } = true;

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

	public User? User { get; set; }
}
