using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Interfaces.Repositories.WorkoutItem;

namespace TrainMe.Data.Repositories.WorkoutItem;

/// <summary>
/// Implementation of basic CRUD operations for WorkoutItem entity
/// </summary>
public class WorkoutItemRepository(AppDbContext context) : IWorkoutItemRepository
{
    public async Task<Core.Entities.WorkoutItem?> GetByIdAsync(int id)
    {
        // Use FindAsync for primary key lookup with Include for navigation property
        return await context.WorkoutItems
            .Include(wi => wi.User)
            .FirstOrDefaultAsync(wi => wi.Id == id);
    }

    public async Task<Core.Entities.WorkoutItem> CreateAsync(Core.Entities.WorkoutItem workoutItem)
    {
        ArgumentNullException.ThrowIfNull(workoutItem);

        // Set audit fields before adding to context
        workoutItem.CreatedAt = DateTime.UtcNow;

        // Use AddAsync for better performance with value generators
        await context.WorkoutItems.AddAsync(workoutItem);
        await context.SaveChangesAsync();

        return workoutItem;
    }

    public async Task<Core.Entities.WorkoutItem> UpdateAsync(Core.Entities.WorkoutItem workoutItem)
    {
        ArgumentNullException.ThrowIfNull(workoutItem);

        // Set audit fields before updating
        workoutItem.UpdatedAt = DateTime.UtcNow;

        // Use Entry to track changes more efficiently
        context.Entry(workoutItem).State = EntityState.Modified;
        await context.SaveChangesAsync();

        return workoutItem;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Use ExecuteDeleteAsync for better performance (EF Core 7+)
        var rowsAffected = await context.WorkoutItems
            .Where(wi => wi.Id == id)
            .ExecuteDeleteAsync();

        return rowsAffected > 0;
    }
}
