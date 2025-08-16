using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories;

namespace TrainMe.Data.Repositories;

/// <summary>
/// Repository implementation for RandomExercise entity
/// </summary>
public class RandomExerciseRepository : IRandomExerciseRepository
{
    private readonly AppDbContext _context;

    public RandomExerciseRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets all random exercises from database
    /// </summary>
    public async Task<IEnumerable<RandomExercise>> GetAllAsync()
    {
        return await _context.RandomExercises
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Gets a random exercise from database
    /// </summary>
    public async Task<RandomExercise?> GetRandomAsync()
    {
        var exercises = await GetAllAsync();
        var exerciseList = exercises.ToList();
        
        if (!exerciseList.Any())
            return null;

        var random = new Random();
        var randomIndex = random.Next(exerciseList.Count);
        return exerciseList[randomIndex];
    }

    /// <summary>
    /// Gets multiple random exercises from database
    /// </summary>
    public async Task<IEnumerable<RandomExercise>> GetRandomAsync(int count)
    {
        var exercises = await GetAllAsync();
        var exerciseList = exercises.ToList();
        
        if (!exerciseList.Any())
            return Enumerable.Empty<RandomExercise>();

        var random = new Random();
        return exerciseList
            .OrderBy(x => random.Next())
            .Take(count)
            .ToList();
    }
}
