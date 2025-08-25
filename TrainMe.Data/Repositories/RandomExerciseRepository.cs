using Microsoft.EntityFrameworkCore;
using TrainMe.Core.Entities;
using TrainMe.Core.Interfaces.Repositories;

namespace TrainMe.Data.Repositories;

/// <summary>
/// Repository xử lý truy cập dữ liệu cho entity RandomExercise
/// </summary>
public class RandomExerciseRepository : IRandomExerciseRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Khởi tạo repository và inject DbContext
    /// </summary>
    /// <param name="context">Ngữ cảnh cơ sở dữ liệu</param>
    public RandomExerciseRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Lấy tất cả bài tập ngẫu nhiên từ cơ sở dữ liệu
    /// </summary>
    public async Task<IEnumerable<RandomExercise>> GetAllAsync()
    {
        return await _context.RandomExercises
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Lấy một bài tập ngẫu nhiên từ cơ sở dữ liệu
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
    /// Lấy nhiều bài tập ngẫu nhiên từ cơ sở dữ liệu
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
