using System.ComponentModel.DataAnnotations;

namespace TrainMe.Core.Entities;

public class User
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string UserName { get; set; } = default!;
    [Required] public string PasswordHash { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [MaxLength(50)] public string Role { get; set; } = "User";
}
