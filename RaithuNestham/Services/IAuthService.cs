using RaithuNestham.DTOs.Auth;

namespace RaithuNestham.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(
        RegisterRequest request);

    Task<AuthResponse?> LoginAsync(
        LoginRequest request);
}
