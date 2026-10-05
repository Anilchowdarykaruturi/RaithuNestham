using RaithuNestham.Data;
using RaithuNestham.DTOs.Auth;
using RaithuNestham.Helpers;
using RaithuNestham.Models;
using RaithuNestham.Repositories.Interfaces;
using RaithuNestham.Services.Interfaces;

namespace RaithuNestham.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        ApplicationDbContext context,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request)
    {
        var existingUser =
            await _userRepository
                .GetByUsernameAsync(request.Username);

        if (existingUser != null)
        {
            throw new Exception(
                "Username already exists.");
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash =
                PasswordHelper.HashPassword(request.Password),
            Role = "Farmer"
        };

        await _userRepository.AddAsync(user);

        var farmer = new Farmer
        {
            UserId = user.Id,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Village = request.Village,
            Mandal = request.Mandal,
            District = request.District,
            State = "Andhra Pradesh"
        };

        _context.Farmers.Add(farmer);

        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Token = _jwtService.GenerateToken(user),
            UserId = user.Id,
            FarmerId = farmer.Id,
            Username = user.Username,
            Role = user.Role
        };
    }

    public async Task<AuthResponse?> LoginAsync(
        LoginRequest request)
    {
        var user =
            await _userRepository
                .GetByUsernameAsync(request.Username);

        if (user == null)
        {
            return null;
        }

        var validPassword =
            PasswordHelper.VerifyPassword(
                request.Password,
                user.PasswordHash);

        if (!validPassword)
        {
            return null;
        }

        return new AuthResponse
        {
            Token = _jwtService.GenerateToken(user),
            UserId = user.Id,
            FarmerId = user.Farmer?.Id ?? 0,
            Username = user.Username,
            Role = user.Role
        };
    }
}
