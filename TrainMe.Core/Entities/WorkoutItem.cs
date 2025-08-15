using System.ComponentModel.DataAnnotations;

namespace TrainMe.Core.Entities;

/// <summary>
/// Enum for days of the week
/// </summary>
public enum Weekday
{
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 7
}

/// <summary>
/// Represents a workout item for a specific day
/// </summary>
public class WorkoutItem
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    
    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;



    [Required]
    public Weekday DayOfWeek { get; set; }

    public int SortOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = default!;
}
