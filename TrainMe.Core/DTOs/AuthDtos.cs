using System.ComponentModel.DataAnnotations;

namespace TrainMe.Core.DTOs;

/// <summary>
/// DTO cho yêu cầu đăng ký tài khoản mới
/// Chứa thông tin cần thiết để tạo tài khoản người dùng
/// </summary>
/// <param name="UserName">Tên đăng nhập (3-100 ký tự)</param>
/// <param name="Password">Mật khẩu (3-8 ký tự)</param>
public record RegisterRequest(
    [Required, StringLength(100, MinimumLength = 3)] string UserName,
    [Required, StringLength(8, MinimumLength = 3)] string Password);

/// <summary>
/// DTO cho yêu cầu đăng nhập
/// Chứa thông tin xác thực người dùng
/// </summary>
/// <param name="UserName">Tên đăng nhập</param>
/// <param name="Password">Mật khẩu</param>
public record LoginRequest(
    [Required] string UserName,
    [Required] string Password);

/// <summary>
/// DTO phản hồi sau khi xác thực thành công
/// Chứa token truy cập và thông tin người dùng
/// </summary>
/// <param name="AccessToken">JWT token để xác thực các request tiếp theo</param>
/// <param name="ExpiresAt">Thời gian hết hạn của token</param>
/// <param name="User">Thông tin chi tiết người dùng</param>
public record AuthResponse(string AccessToken, DateTime ExpiresAt, UserDto User);

/// <summary>
/// DTO thông tin chi tiết người dùng
/// Sử dụng để trả về thông tin đầy đủ của user
/// </summary>
/// <param name="Id">ID duy nhất của người dùng</param>
/// <param name="UserName">Tên đăng nhập</param>
/// <param name="Role">Vai trò của người dùng (User, Admin, etc.)</param>
/// <param name="CreatedAt">Thời gian tạo tài khoản</param>
/// <param name="WorkoutItems">Danh sách bài tập của người dùng (tùy chọn)</param>
public record UserDto(int Id, string UserName, string Role, DateTime CreatedAt, ICollection<WorkoutItemSummaryDto>? WorkoutItems = null);

/// <summary>
/// DTO thông tin tóm tắt người dùng
/// Sử dụng khi chỉ cần thông tin cơ bản
/// </summary>
/// <param name="Id">ID duy nhất của người dùng</param>
/// <param name="UserName">Tên đăng nhập</param>
/// <param name="Role">Vai trò của người dùng</param>
public record UserSummaryDto(int Id, string UserName, string Role);

/// <summary>
/// DTO cho yêu cầu tạo người dùng mới (dành cho Admin)
/// Cho phép chỉ định role khi tạo user
/// </summary>
/// <param name="UserName">Tên đăng nhập (3-100 ký tự)</param>
/// <param name="Password">Mật khẩu (3-8 ký tự)</param>
/// <param name="Role">Vai trò của người dùng (mặc định là "User")</param>
public record CreateUserRequest(
    [Required, StringLength(100, MinimumLength = 3)] string UserName,
    [Required, StringLength(8, MinimumLength = 3)] string Password,
    string Role = "User");

/// <summary>
/// DTO cho yêu cầu cập nhật thông tin người dùng
/// Cho phép thay đổi tên đăng nhập và role
/// </summary>
/// <param name="UserName">Tên đăng nhập mới (3-100 ký tự)</param>
/// <param name="Role">Vai trò mới (mặc định là "User")</param>
public record UpdateUserRequest(
    [Required, StringLength(100, MinimumLength = 3)] string UserName,
    string Role = "User");
