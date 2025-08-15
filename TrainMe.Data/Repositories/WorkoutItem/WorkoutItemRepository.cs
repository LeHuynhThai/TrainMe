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
        return await context.WorkoutItems
            .Include(wi => wi.User)
            .FirstOrDefaultAsync(wi => wi.Id == id);
    }

    public async Task<Core.Entities.WorkoutItem> CreateAsync(Core.Entities.WorkoutItem workoutItem)
    {
        ArgumentNullException.ThrowIfNull(workoutItem);
        
        workoutItem.CreatedAt = DateTime.UtcNow;
        context.WorkoutItems.Add(workoutItem);
        await context.SaveChangesAsync();
        
        return workoutItem;
    }

    public async Task<Core.Entities.WorkoutItem> UpdateAsync(Core.Entities.WorkoutItem workoutItem)
    {
        ArgumentNullException.ThrowIfNull(workoutItem);
        
        workoutItem.UpdatedAt = DateTime.UtcNow;
        context.WorkoutItems.Update(workoutItem);
        await context.SaveChangesAsync();
        
        return workoutItem;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var workoutItem = await context.WorkoutItems.FindAsync(id);
        if (workoutItem == null)
            return false;

        context.WorkoutItems.Remove(workoutItem);
        await context.SaveChangesAsync();
        return true;
    }
}
