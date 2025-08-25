using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Interfaces.Repositories.User;

namespace TrainMe.Data.Repositories.User;

/// <summary>
/// Repository truy cập dữ liệu cho người dùng (User)
/// </summary>
public class UserRepository(AppDbContext context) : IUserRepository
{
    /// <summary>
    /// Lấy người dùng theo Id
    /// </summary>
    /// <param name="id">Id người dùng</param>
    /// <returns>Đối tượng User hoặc null nếu không tìm thấy</returns>
    public async Task<Core.Entities.User?> GetByIdAsync(int id) => await context.Users.FindAsync(id);

    /// <summary>
    /// Lấy người dùng theo tên đăng nhập (UserName)
    /// </summary>
    /// <param name="userName">Tên đăng nhập</param>
    /// <returns>Đối tượng User hoặc null nếu không tìm thấy</returns>
    public async Task<Core.Entities.User?> GetByUserNameAsync(string userName) =>
        await context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

    /// <summary>
    /// Kiểm tra người dùng có tồn tại theo tên đăng nhập hay không
    /// </summary>
    /// <param name="userName">Tên đăng nhập</param>
    /// <returns>True nếu tồn tại, ngược lại False</returns>
    public async Task<bool> ExistsByUserNameAsync(string userName) =>
        await context.Users.AnyAsync(u => u.UserName == userName);

    /// <summary>
    /// Tạo mới người dùng
    /// </summary>
    /// <param name="user">Đối tượng người dùng cần tạo</param>
    /// <returns>Đối tượng người dùng sau khi lưu</returns>
    public async Task<Core.Entities.User> CreateAsync(Core.Entities.User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
}
