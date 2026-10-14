namespace RaithuNestham.DTOs.Auth;

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Village { get; set; } = string.Empty;

    public string Mandal { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;
}