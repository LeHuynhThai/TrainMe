using System.ComponentModel.DataAnnotations;

namespace TrainMe.Core.Entities;

/// <summary>
/// Entity đại diện cho các bài tập ngẫu nhiên trong hệ thống
/// Được sử dụng để gợi ý bài tập cho người dùng khi họ cần ý tưởng mới
/// </summary>
public class RandomExercise
{
    /// <summary>
    /// ID duy nhất của bài tập ngẫu nhiên (Primary Key)
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tên của bài tập (ví dụ: "Push-up", "Squat", "Plank", "Jumping Jacks")
    /// Bắt buộc và có độ dài tối đa 200 ký tự
    /// Đây là tên bài tập sẽ được hiển thị cho người dùng khi gợi ý
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;
}
