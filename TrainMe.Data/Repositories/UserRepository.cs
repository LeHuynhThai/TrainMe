using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories;

namespace TrainMe.Data.Repositories;

/// <summary>
/// Repository implementation for User entity operations
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<User?> GetByIdAsync(int id) =>
        await _context.Users.FindAsync(id);

    public async Task<User?> GetByUserNameAsync(string userName) =>
        await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

    public async Task<User> CreateAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> ExistsByUserNameAsync(string userName) =>
        await _context.Users.AnyAsync(u => u.UserName == userName);
}
