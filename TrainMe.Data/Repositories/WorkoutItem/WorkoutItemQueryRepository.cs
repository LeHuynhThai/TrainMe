using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories.WorkoutItem;

namespace TrainMe.Data.Repositories.WorkoutItem;

/// <summary>
/// Implementation of query operations for WorkoutItem entity
/// </summary>
public class WorkoutItemQueryRepository(AppDbContext context) : IWorkoutItemQueryRepository
{
    public async Task<IEnumerable<Core.Entities.WorkoutItem>> GetByUserIdAsync(int userId)
    {
        return await context.WorkoutItems
            .Where(wi => wi.UserId == userId)
            .OrderBy(wi => wi.DayOfWeek)
            .ThenBy(wi => wi.SortOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<Core.Entities.WorkoutItem>> GetByUserIdAndDayAsync(int userId, Weekday dayOfWeek)
    {
        return await context.WorkoutItems
            .Where(wi => wi.UserId == userId && wi.DayOfWeek == dayOfWeek)
            .OrderBy(wi => wi.SortOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<Core.Entities.WorkoutItem>> GetByUserIdOrderedAsync(int userId)
    {
        return await context.WorkoutItems
            .Where(wi => wi.UserId == userId)
            .OrderBy(wi => wi.DayOfWeek)
            .ThenBy(wi => wi.SortOrder)
            .ThenBy(wi => wi.Name)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int userId, string name, Weekday dayOfWeek, int? excludeId = null)
    {
        var query = context.WorkoutItems
            .Where(wi => wi.UserId == userId && 
                        wi.Name == name && 
                        wi.DayOfWeek == dayOfWeek);

        if (excludeId.HasValue)
        {
            query = query.Where(wi => wi.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
