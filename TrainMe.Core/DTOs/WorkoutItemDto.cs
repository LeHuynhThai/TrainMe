using System.ComponentModel.DataAnnotations;
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
    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    [Required]
    public Weekday DayOfWeek { get; set; }

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; } = 0;
}

/// <summary>
/// DTO for updating a WorkoutItem
/// </summary>
public class UpdateWorkoutItemRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    [Required]
    public Weekday DayOfWeek { get; set; }

    [Range(0, int.MaxValue)]
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

/// <summary>
/// DTO for reordering workout items
/// </summary>
public class ReorderWorkoutItemsRequest
{
    public Weekday DayOfWeek { get; set; }
    public Dictionary<int, int> ItemSortOrders { get; set; } = new();
}

/// <summary>
/// DTO for duplicating workout item to another day
/// </summary>
public class DuplicateWorkoutItemRequest
{
    public Weekday TargetDay { get; set; }
}
