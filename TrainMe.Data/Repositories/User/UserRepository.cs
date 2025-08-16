using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Interfaces.Repositories.User;

namespace TrainMe.Data.Repositories.User;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<Core.Entities.User?> GetByIdAsync(int id) => await context.Users.FindAsync(id);

    public async Task<Core.Entities.User?> GetByUserNameAsync(string userName) =>
        await context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

    public async Task<bool> ExistsByUserNameAsync(string userName) =>
        await context.Users.AnyAsync(u => u.UserName == userName);

    public async Task<Core.Entities.User> CreateAsync(Core.Entities.User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
}
