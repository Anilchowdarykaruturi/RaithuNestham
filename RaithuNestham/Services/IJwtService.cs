using RaithuNestham.Models;

namespace RaithuNestham.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
