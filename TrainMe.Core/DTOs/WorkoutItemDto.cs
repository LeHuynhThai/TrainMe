using System.ComponentModel.DataAnnotations;
using TrainMe.Core.Entities;

namespace TrainMe.Core.DTOs;

/// <summary>
/// DTO chứa thông tin chi tiết của một bài tập
/// Sử dụng để trả về đầy đủ thông tin bài tập cho client
/// </summary>
public class WorkoutItemDto
{
    /// <summary>ID duy nhất của bài tập</summary>
    public int Id { get; set; }

    /// <summary>ID của người dùng sở hữu bài tập</summary>
    public int UserId { get; set; }

    /// <summary>Tên bài tập</summary>
    public string Name { get; set; } = default!;

    /// <summary>Ngày trong tuần thực hiện bài tập</summary>
    public Weekday DayOfWeek { get; set; }

    /// <summary>Thứ tự sắp xếp trong ngày (để hiển thị theo thứ tự)</summary>
    public int SortOrder { get; set; }

    /// <summary>Thời gian tạo bài tập</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Thời gian cập nhật lần cuối (nếu có)</summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO cho yêu cầu tạo bài tập mới
/// Chứa thông tin cần thiết để tạo một bài tập
/// </summary>
public class CreateWorkoutItemRequest
{
    /// <summary>
    /// Tên bài tập (bắt buộc, tối đa 200 ký tự)
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Ngày trong tuần thực hiện bài tập (bắt buộc)
    /// </summary>
    [Required]
    public Weekday DayOfWeek { get; set; }

    /// <summary>
    /// Thứ tự sắp xếp trong ngày (mặc định là 0)
    /// Số càng nhỏ sẽ hiển thị trước
    /// </summary>
    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; } = 0;
}

/// <summary>
/// DTO cho yêu cầu cập nhật bài tập
/// Chứa thông tin mới để cập nhật bài tập hiện có
/// </summary>
public class UpdateWorkoutItemRequest
{
    /// <summary>
    /// Tên bài tập mới (bắt buộc, tối đa 200 ký tự)
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    /// <summary>
    /// Ngày trong tuần mới để thực hiện bài tập (bắt buộc)
    /// </summary>
    [Required]
    public Weekday DayOfWeek { get; set; }

    /// <summary>
    /// Thứ tự sắp xếp mới trong ngày
    /// </summary>
    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }
}

/// <summary>
/// DTO tóm tắt thông tin bài tập (phiên bản nhẹ)
/// Sử dụng khi chỉ cần thông tin cơ bản, không cần chi tiết đầy đủ
/// </summary>
public class WorkoutItemSummaryDto
{
    /// <summary>ID duy nhất của bài tập</summary>
    public int Id { get; set; }

    /// <summary>Tên bài tập</summary>
    public string Name { get; set; } = default!;

    /// <summary>Ngày trong tuần thực hiện</summary>
    public Weekday DayOfWeek { get; set; }

    /// <summary>Thứ tự sắp xếp trong ngày</summary>
    public int SortOrder { get; set; }
}

/// <summary>
/// DTO cho yêu cầu sắp xếp lại thứ tự các bài tập
/// Cho phép thay đổi thứ tự hiển thị của các bài tập trong một ngày
/// </summary>
public class ReorderWorkoutItemsRequest
{
    /// <summary>
    /// Ngày trong tuần cần sắp xếp lại
    /// </summary>
    public Weekday DayOfWeek { get; set; }

    /// <summary>
    /// Dictionary chứa mapping từ ID bài tập sang thứ tự mới
    /// Key: ID của bài tập, Value: Thứ tự mới
    /// </summary>
    public Dictionary<int, int> ItemSortOrders { get; set; } = new();
}

/// <summary>
/// DTO cho yêu cầu sao chép bài tập sang ngày khác
/// Cho phép duplicate một bài tập từ ngày này sang ngày khác
/// </summary>
public class DuplicateWorkoutItemRequest
{
    /// <summary>
    /// Ngày đích để sao chép bài tập tới
    /// </summary>
    public Weekday TargetDay { get; set; }
}
