// Interface này định nghĩa các phương thức thao tác với dữ liệu người dùng trong hệ thống
namespace TrainMe.Core.Interfaces.Repositories.User;

/// <summary>
/// Interface quản lý thao tác với bảng User trong database
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Lấy thông tin người dùng theo Id (nếu có)
    /// </summary>
    /// <param name="id">Id của người dùng</param>
    /// <returns>Thông tin người dùng hoặc null nếu không tồn tại</returns>
    Task<Entities.User?> GetByIdAsync(int id);
    /// <summary>
    /// Lấy thông tin người dùng theo tên đăng nhập (UserName)
    /// </summary>
    /// <param name="userName">Tên đăng nhập</param>
    /// <returns>Thông tin người dùng hoặc null nếu không tồn tại</returns>
    Task<Entities.User?> GetByUserNameAsync(string userName);
    /// <summary>
    /// Tạo mới một người dùng
    /// </summary>
    /// <param name="user">Đối tượng User cần tạo</param>
    /// <returns>Thông tin người dùng vừa tạo</returns>
    Task<Entities.User> CreateAsync(Entities.User user);
    /// <summary>
    /// Kiểm tra tên đăng nhập đã tồn tại trong hệ thống chưa
    /// </summary>
    /// <param name="userName">Tên đăng nhập</param>
    /// <returns>True nếu đã tồn tại, False nếu chưa</returns>
    Task<bool> ExistsByUserNameAsync(string userName);
}
