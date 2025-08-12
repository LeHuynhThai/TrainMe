using TrainMe.Core.Entities;

namespace TrainMe.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUserNameAsync(string userName);
        Task<User> CreateAsync(User user);
        Task<bool> ExistsByUserNameAsync(string userName);
    }
}
