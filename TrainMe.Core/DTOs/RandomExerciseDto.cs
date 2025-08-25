namespace TrainMe.Core.DTOs;

/// <summary>
/// DTO cho bài tập ngẫu nhiên
/// Chứa thông tin cơ bản của một bài tập trong hệ thống gợi ý
/// </summary>
public class RandomExerciseDto
{
    /// <summary>
    /// ID duy nhất của bài tập ngẫu nhiên
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tên bài tập (ví dụ: "Push-up", "Squat", "Plank")
    /// </summary>
    public string Name { get; set; } = default!;
}
