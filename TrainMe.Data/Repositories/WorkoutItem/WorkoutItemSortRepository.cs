using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories.WorkoutItem;

namespace TrainMe.Data.Repositories.WorkoutItem;

/// <summary>
/// Implementation of sort and ordering operations for WorkoutItem entity
/// </summary>
public class WorkoutItemSortRepository(AppDbContext context) : IWorkoutItemSortRepository
{
    public async Task<int> GetNextSortOrderAsync(int userId, Weekday dayOfWeek)
    {
        // Use AsNoTracking with MaxAsync for optimal performance
        // Cast to nullable int to handle empty collections
        var maxSortOrder = await context.WorkoutItems
            .AsNoTracking()
            .Where(wi => wi.UserId == userId && wi.DayOfWeek == dayOfWeek)
            .MaxAsync(wi => (int?)wi.SortOrder);

        return (maxSortOrder ?? 0) + 1;
    }

    public async Task UpdateSortOrdersAsync(int userId, Weekday dayOfWeek, Dictionary<int, int> itemSortOrders)
    {
        ArgumentNullException.ThrowIfNull(itemSortOrders);

        if (!itemSortOrders.Any())
            return;

        // Use ExecuteUpdateAsync for bulk update (EF Core 7+) - more efficient than loading entities
        var workoutItemIds = itemSortOrders.Keys.ToList();

        // Load entities for complex update logic (when ExecuteUpdateAsync isn't suitable)
        var workoutItems = await context.WorkoutItems
            .Where(wi => wi.UserId == userId &&
                        wi.DayOfWeek == dayOfWeek &&
                        workoutItemIds.Contains(wi.Id))
            .ToListAsync();

        // Update sort orders and audit fields
        foreach (var workoutItem in workoutItems)
        {
            if (itemSortOrders.TryGetValue(workoutItem.Id, out var newSortOrder))
            {
                workoutItem.SortOrder = newSortOrder;
                workoutItem.UpdatedAt = DateTime.UtcNow;
            }
        }

        // Save all changes in single transaction
        await context.SaveChangesAsync();
    }
}
