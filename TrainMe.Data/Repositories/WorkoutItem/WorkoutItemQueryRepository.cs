using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories.WorkoutItem;

namespace TrainMe.Data.Repositories.WorkoutItem;

/// <summary>
/// Repository thực hiện các truy vấn cho entity WorkoutItem
/// </summary>
public class WorkoutItemQueryRepository(AppDbContext context) : IWorkoutItemQueryRepository
{
    /// <summary>
    /// Lấy danh sách bài tập của một người dùng
    /// </summary>
    /// <param name="userId">Id người dùng</param>
    /// <returns>Danh sách bài tập đã được sắp xếp theo thứ trong tuần và thứ tự hiển thị</returns>
    public async Task<IEnumerable<Core.Entities.WorkoutItem>> GetByUserIdAsync(int userId)
    {
        // Sử dụng AsNoTracking cho các truy vấn chỉ đọc để cải thiện hiệu năng
        return await context.WorkoutItems
            .AsNoTracking()
            .Where(wi => wi.UserId == userId)
            .OrderBy(wi => wi.DayOfWeek)
            .ThenBy(wi => wi.SortOrder)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách bài tập của người dùng theo một ngày cụ thể trong tuần
    /// </summary>
    /// <param name="userId">Id người dùng</param>
    /// <param name="dayOfWeek">Ngày trong tuần</param>
    /// <returns>Danh sách bài tập đã sắp xếp theo thứ tự hiển thị</returns>
    public async Task<IEnumerable<Core.Entities.WorkoutItem>> GetByUserIdAndDayAsync(int userId, Weekday dayOfWeek)
    {
        // Sử dụng AsNoTracking và bộ lọc tổng hợp để tối ưu hiệu năng truy vấn
        return await context.WorkoutItems
            .AsNoTracking()
            .Where(wi => wi.UserId == userId && wi.DayOfWeek == dayOfWeek)
            .OrderBy(wi => wi.SortOrder)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách bài tập của người dùng với thứ tự hiển thị đầy đủ
    /// </summary>
    /// <param name="userId">Id người dùng</param>
    /// <returns>Danh sách bài tập đã sắp xếp theo ngày trong tuần, thứ tự và tên</returns>
    public async Task<IEnumerable<Core.Entities.WorkoutItem>> GetByUserIdOrderedAsync(int userId)
    {
        // Sử dụng AsNoTracking với sắp xếp đầy đủ phục vụ mục đích hiển thị
        return await context.WorkoutItems
            .AsNoTracking()
            .Where(wi => wi.UserId == userId)
            .OrderBy(wi => wi.DayOfWeek)
            .ThenBy(wi => wi.SortOrder)
            .ThenBy(wi => wi.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Kiểm tra bài tập có tồn tại theo tên và ngày trong tuần của một người dùng hay không
    /// </summary>
    /// <param name="userId">Id người dùng</param>
    /// <param name="name">Tên bài tập</param>
    /// <param name="dayOfWeek">Ngày trong tuần</param>
    /// <param name="excludeId">Loại trừ một Id (dùng trong tình huống cập nhật)</param>
    /// <returns>True nếu tồn tại, ngược lại False</returns>
    public async Task<bool> ExistsAsync(int userId, string name, Weekday dayOfWeek, int? excludeId = null)
    {
        // Sử dụng AsNoTracking cho kiểm tra tồn tại kèm điều kiện
        var query = context.WorkoutItems
            .AsNoTracking()
            .Where(wi => wi.UserId == userId &&
                        wi.Name == name &&
                        wi.DayOfWeek == dayOfWeek);

        // Áp dụng điều kiện loại trừ nếu được cung cấp (phục vụ tình huống cập nhật)
        if (excludeId.HasValue)
        {
            query = query.Where(wi => wi.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
