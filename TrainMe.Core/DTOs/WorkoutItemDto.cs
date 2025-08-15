using TrainMe.Core.Entities;

namespace TrainMe.Core.DTOs;

/// <summary>
/// DTO for WorkoutItem details
/// </summary>
public class WorkoutItemDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = default!;
    public string? Notes { get; set; }
    public Weekday DayOfWeek { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new WorkoutItem
/// </summary>
public class CreateWorkoutItemRequest
{
    public string Name { get; set; } = default!;
    public string? Notes { get; set; }
    public Weekday DayOfWeek { get; set; }
    public int SortOrder { get; set; } = 0;
}

/// <summary>
/// DTO for updating a WorkoutItem
/// </summary>
public class UpdateWorkoutItemRequest
{
    public string Name { get; set; } = default!;
    public string? Notes { get; set; }
    public Weekday DayOfWeek { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>
/// DTO for WorkoutItem summary (lightweight)
/// </summary>
public class WorkoutItemSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public Weekday DayOfWeek { get; set; }
    public int SortOrder { get; set; }
}
