using System.ComponentModel.DataAnnotations;

namespace TrainMe.Core.Entities;

/// <summary>
/// Enum định nghĩa các ngày trong tuần
/// Sử dụng để xác định ngày thực hiện bài tập
/// </summary>
public enum Weekday
{
    /// <summary>Thứ Hai</summary>
    Monday = 1,
    /// <summary>Thứ Ba</summary>
    Tuesday = 2,
    /// <summary>Thứ Tư</summary>
    Wednesday = 3,
    /// <summary>Thứ Năm</summary>
    Thursday = 4,
    /// <summary>Thứ Sáu</summary>
    Friday = 5,
    /// <summary>Thứ Bảy</summary>
    Saturday = 6,
    /// <summary>Chủ Nhật</summary>
    Sunday = 7
}

/// <summary>
/// Entity đại diện cho một bài tập trong lịch tập luyện
/// Mỗi bài tập thuộc về một người dùng và được thực hiện vào một ngày cụ thể trong tuần
/// </summary>
public class WorkoutItem
{
    /// <summary>
    /// ID duy nhất của bài tập (Primary Key)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID của người dùng sở hữu bài tập này (Foreign Key)
    /// Bắt buộc phải có để xác định chủ sở hữu
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Tên của bài tập (ví dụ: "Push-up 3 sets", "Chạy bộ 30 phút")
    /// Bắt buộc và có độ dài tối đa 200 ký tự
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Ngày trong tuần thực hiện bài tập
    /// Sử dụng enum Weekday để đảm bảo tính nhất quán
    /// </summary>
    [Required]
    public Weekday DayOfWeek { get; set; }

    /// <summary>
    /// Thứ tự sắp xếp của bài tập trong ngày
    /// Số càng nhỏ sẽ hiển thị trước, mặc định là 0
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Thời gian tạo bài tập
    /// Mặc định là thời gian hiện tại khi tạo
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời gian cập nhật lần cuối
    /// Null nếu chưa từng được cập nhật
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties - Các thuộc tính điều hướng

    /// <summary>
    /// Tham chiếu đến người dùng sở hữu bài tập này
    /// Sử dụng virtual để hỗ trợ lazy loading của Entity Framework
    /// </summary>
    public virtual User User { get; set; } = default!;
}
