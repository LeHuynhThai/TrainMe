using System.ComponentModel.DataAnnotations;

namespace TrainMe.Core.DTOs;

public record RegisterRequest(
    [Required, StringLength(100, MinimumLength = 3)] string UserName,
    [Required, StringLength(100, MinimumLength = 6)] string Password);

public record LoginRequest(
    [Required] string UserName,
    [Required] string Password);

public record AuthResponse(string AccessToken, DateTime ExpiresAt, UserDto User);

public record UserDto(int Id, string UserName, string Role, DateTime CreatedAt);
