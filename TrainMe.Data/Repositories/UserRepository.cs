using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories;

namespace TrainMe.Data.Repositories
{
    /// <summary>
    /// Implementation của IUserRepository - thực hiện các thao tác CRUD với User trong database
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tìm user theo ID sử dụng Entity Framework FindAsync
        /// </summary>
        /// <param name="id">ID của user</param>
        /// <returns>User object hoặc null</returns>
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        /// <summary>
        /// Tìm user theo UserName sử dụng LINQ FirstOrDefaultAsync
        /// </summary>
        /// <param name="userName">Tên đăng nhập</param>
        /// <returns>User object hoặc null</returns>
        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }

        /// <summary>
        /// Tạo user mới - thêm vào DbSet và lưu vào database
        /// </summary>
        /// <param name="user">User object cần tạo</param>
        /// <returns>User object với ID đã được generate</returns>
        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của UserName sử dụng LINQ AnyAsync
        /// </summary>
        /// <param name="userName">Tên đăng nhập cần kiểm tra</param>
        /// <returns>True nếu tồn tại, False nếu không</returns>
        public async Task<bool> ExistsByUserNameAsync(string userName)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userName);
        }
    }
}
