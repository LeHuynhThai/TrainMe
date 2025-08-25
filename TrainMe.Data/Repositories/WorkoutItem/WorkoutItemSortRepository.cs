using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories.WorkoutItem;

namespace TrainMe.Data.Repositories.WorkoutItem;

/// <summary>
/// Repository thực hiện các thao tác liên quan đến sắp xếp/thứ tự cho entity WorkoutItem
/// </summary>
public class WorkoutItemSortRepository(AppDbContext context) : IWorkoutItemSortRepository
{
    /// <summary>
    /// Lấy giá trị SortOrder tiếp theo cho một ngày trong tuần của người dùng
    /// </summary>
    /// <param name="userId">Id người dùng</param>
    /// <param name="dayOfWeek">Ngày trong tuần</param>
    /// <returns>Giá trị SortOrder kế tiếp (bắt đầu từ 1 nếu chưa có dữ liệu)</returns>
    public async Task<int> GetNextSortOrderAsync(int userId, Weekday dayOfWeek)
    {
        // Sử dụng AsNoTracking kết hợp MaxAsync để tối ưu hiệu năng
        // Ép kiểu về int? để xử lý trường hợp tập rỗng (không có phần tử)
        var maxSortOrder = await context.WorkoutItems
            .AsNoTracking()
            .Where(wi => wi.UserId == userId && wi.DayOfWeek == dayOfWeek)
            .MaxAsync(wi => (int?)wi.SortOrder);

        return (maxSortOrder ?? 0) + 1;
    }

    /// <summary>
    /// Cập nhật thứ tự sắp xếp (SortOrder) cho các bài tập theo ngày trong tuần
    /// </summary>
    /// <param name="userId">Id người dùng</param>
    /// <param name="dayOfWeek">Ngày trong tuần</param>
    /// <param name="itemSortOrders">Bản đồ (Id -> SortOrder mới)</param>
    public async Task UpdateSortOrdersAsync(int userId, Weekday dayOfWeek, Dictionary<int, int> itemSortOrders)
    {
        ArgumentNullException.ThrowIfNull(itemSortOrders);

        if (!itemSortOrders.Any())
            return;

        // Có thể dùng ExecuteUpdateAsync để cập nhật hàng loạt (EF Core 7+) — hiệu năng cao hơn so với load entity
        var workoutItemIds = itemSortOrders.Keys.ToList();

        // Ở đây cần logic cập nhật phức tạp nên load entity để xử lý (khi không dùng được ExecuteUpdateAsync)
        var workoutItems = await context.WorkoutItems
            .Where(wi => wi.UserId == userId &&
                        wi.DayOfWeek == dayOfWeek &&
                        workoutItemIds.Contains(wi.Id))
            .ToListAsync();

        // Cập nhật SortOrder và trường audit (UpdatedAt)
        foreach (var workoutItem in workoutItems)
        {
            if (itemSortOrders.TryGetValue(workoutItem.Id, out var newSortOrder))
            {
                workoutItem.SortOrder = newSortOrder;
                workoutItem.UpdatedAt = DateTime.UtcNow;
            }
        }

        // Lưu tất cả thay đổi trong một giao dịch
        await context.SaveChangesAsync();
    }
}
