using RaithuNestham.Models;

namespace RaithuNestham.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);

    Task<User> AddAsync(User user);
}