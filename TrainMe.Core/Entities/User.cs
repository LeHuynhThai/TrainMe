using System.ComponentModel.DataAnnotations;

namespace TrainMe.Core.Entities;

/// <summary>
/// Entity đại diện cho người dùng trong hệ thống
/// Chứa thông tin cơ bản và các thuộc tính liên quan đến authentication
/// </summary>
public class User
{
    /// <summary>
    /// ID duy nhất của người dùng (Primary Key)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tên đăng nhập duy nhất của người dùng
    /// Bắt buộc và có độ dài tối đa 100 ký tự
    /// </summary>
    [Required, MaxLength(100)]
    public string UserName { get; set; } = default!;

    /// <summary>
    /// Mật khẩu đã được hash để bảo mật
    /// Không bao giờ lưu mật khẩu dạng plain text
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = default!;

    /// <summary>
    /// Thời gian tạo tài khoản
    /// Mặc định là thời gian hiện tại khi tạo
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Vai trò của người dùng trong hệ thống
    /// Mặc định là "User", có thể là "Admin" hoặc các role khác
    /// </summary>
    [MaxLength(50)]
    public string Role { get; set; } = "User";

    // Navigation properties - Các thuộc tính điều hướng

    /// <summary>
    /// Danh sách các bài tập thuộc về người dùng này
    /// Sử dụng virtual để hỗ trợ lazy loading của Entity Framework
    /// </summary>
    public virtual ICollection<WorkoutItem> WorkoutItems { get; set; } = new List<WorkoutItem>();
}
