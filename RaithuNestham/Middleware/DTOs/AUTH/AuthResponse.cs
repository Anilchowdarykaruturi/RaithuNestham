namespace RaithuNestham.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;

    public int UserId { get; set; }

    public int FarmerId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}