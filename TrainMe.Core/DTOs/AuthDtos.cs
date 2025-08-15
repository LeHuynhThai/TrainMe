using System.ComponentModel.DataAnnotations;

namespace TrainMe.Core.DTOs;

public record RegisterRequest(
    [Required, StringLength(100, MinimumLength = 3)] string UserName,
    [Required, StringLength(8, MinimumLength = 3)] string Password);

public record LoginRequest(
    [Required] string UserName,
    [Required] string Password);

public record AuthResponse(string AccessToken, DateTime ExpiresAt, UserDto User);

public record UserDto(int Id, string UserName, string Role, DateTime CreatedAt, ICollection<WorkoutItemSummaryDto>? WorkoutItems = null);

public record UserSummaryDto(int Id, string UserName, string Role);

public record CreateUserRequest(
    [Required, StringLength(100, MinimumLength = 3)] string UserName,
    [Required, StringLength(8, MinimumLength = 3)] string Password,
    string Role = "User");

public record UpdateUserRequest(
    [Required, StringLength(100, MinimumLength = 3)] string UserName,
    string Role = "User");
