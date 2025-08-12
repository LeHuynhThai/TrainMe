using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories;

namespace TrainMe.Data.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id) => await context.Users.FindAsync(id);
    public async Task<User?> GetByUserNameAsync(string userName) =>
        await context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
    public async Task<bool> ExistsByUserNameAsync(string userName) =>
        await context.Users.AnyAsync(u => u.UserName == userName);

    public async Task<User> CreateAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
}
