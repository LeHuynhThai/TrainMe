using TrainMe.Core.Entities;

namespace TrainMe.Data.Seed;

/// <summary>
/// Seed data for RandomExercise entity - 10 home exercises
/// </summary>
public static class RandomExerciseSeedData
{
    public static List<RandomExercise> GetSeedData()
    {
        return new List<RandomExercise>
        {
            new RandomExercise { Id = 1, Name = "Push-ups" },
            new RandomExercise { Id = 2, Name = "Squats" },
            new RandomExercise { Id = 3, Name = "Jumping Jacks" },
            new RandomExercise { Id = 4, Name = "Plank" },
            new RandomExercise { Id = 5, Name = "Burpees" },
            new RandomExercise { Id = 6, Name = "Lunges" },
            new RandomExercise { Id = 7, Name = "Mountain Climbers" },
            new RandomExercise { Id = 8, Name = "High Knees" },
            new RandomExercise { Id = 9, Name = "Bicycle Crunches" },
            new RandomExercise { Id = 10, Name = "Wall Sit" }
        };
    }
}
