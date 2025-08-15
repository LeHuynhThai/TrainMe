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
        var maxSortOrder = await context.WorkoutItems
            .Where(wi => wi.UserId == userId && wi.DayOfWeek == dayOfWeek)
            .MaxAsync(wi => (int?)wi.SortOrder);

        return (maxSortOrder ?? 0) + 1;
    }

    public async Task UpdateSortOrdersAsync(int userId, Weekday dayOfWeek, Dictionary<int, int> itemSortOrders)
    {
        ArgumentNullException.ThrowIfNull(itemSortOrders);

        if (!itemSortOrders.Any())
            return;

        var workoutItemIds = itemSortOrders.Keys.ToList();
        var workoutItems = await context.WorkoutItems
            .Where(wi => wi.UserId == userId && 
                        wi.DayOfWeek == dayOfWeek && 
                        workoutItemIds.Contains(wi.Id))
            .ToListAsync();

        foreach (var workoutItem in workoutItems)
        {
            if (itemSortOrders.TryGetValue(workoutItem.Id, out var newSortOrder))
            {
                workoutItem.SortOrder = newSortOrder;
                workoutItem.UpdatedAt = DateTime.UtcNow;
            }
        }

        await context.SaveChangesAsync();
    }
}
