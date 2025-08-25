using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Interfaces.Repositories.WorkoutItem;

namespace TrainMe.Data.Repositories.WorkoutItem;

/// <summary>
/// Repository thực hiện các thao tác CRUD cơ bản cho entity WorkoutItem
/// </summary>
public class WorkoutItemRepository(AppDbContext context) : IWorkoutItemRepository
{
    public async Task<Core.Entities.WorkoutItem?> GetByIdAsync(int id)
    {
        // Truy vấn kèm Include(User) để lấy theo khoá chính và load thuộc tính điều hướng
        return await context.WorkoutItems
            .Include(wi => wi.User)
            .FirstOrDefaultAsync(wi => wi.Id == id);
    }

    public async Task<Core.Entities.WorkoutItem> CreateAsync(Core.Entities.WorkoutItem workoutItem)
    {
        ArgumentNullException.ThrowIfNull(workoutItem);

        // Thiết lập các trường audit trước khi thêm vào DbContext
        workoutItem.CreatedAt = DateTime.UtcNow;

        // Dùng AddAsync để có hiệu năng tốt hơn khi sử dụng value generator (EF)
        await context.WorkoutItems.AddAsync(workoutItem);
        await context.SaveChangesAsync();
        
        return workoutItem;
    }

    public async Task<Core.Entities.WorkoutItem> UpdateAsync(Core.Entities.WorkoutItem workoutItem)
    {
        ArgumentNullException.ThrowIfNull(workoutItem);

        // Thiết lập các trường audit trước khi cập nhật
        workoutItem.UpdatedAt = DateTime.UtcNow;

        // Sử dụng Entry để đánh dấu trạng thái Modified giúp theo dõi thay đổi hiệu quả
        context.Entry(workoutItem).State = EntityState.Modified;
        await context.SaveChangesAsync();
        
        return workoutItem;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Sử dụng ExecuteDeleteAsync để xoá trực tiếp trên DB (EF Core 7+), hiệu năng tốt hơn
        var rowsAffected = await context.WorkoutItems
            .Where(wi => wi.Id == id)
            .ExecuteDeleteAsync();

        return rowsAffected > 0;
    }
}
