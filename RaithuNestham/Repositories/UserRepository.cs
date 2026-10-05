using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Models;
using RaithuNestham.Repositories.Interfaces;

namespace RaithuNestham.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(
        string username)
    {
        return await _context.Users
            .Include(x => x.Farmer)
            .FirstOrDefaultAsync(
                x => x.Username == username);
    }

    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        return user;
    }
}