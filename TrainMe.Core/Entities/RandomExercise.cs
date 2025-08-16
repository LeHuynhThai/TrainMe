using System.ComponentModel.DataAnnotations;

namespace TrainMe.Core.Entities;

/// <summary>
/// Represents a random exercise that can be suggested to users
/// </summary>
public class RandomExercise
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;
}
