namespace TrainMe.Core.Interfaces.Repositories.User;

public interface IUserRepository
{
    Task<Entities.User?> GetByIdAsync(int id);
    Task<Entities.User?> GetByUserNameAsync(string userName);
    Task<Entities.User> CreateAsync(Entities.User user);
    Task<bool> ExistsByUserNameAsync(string userName);
}
