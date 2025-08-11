using System.ComponentModel.DataAnnotations;

namespace TrainMe.Core.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }
        [Required]
        public string PasswordHash { get; set; }
    }
}
