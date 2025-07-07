using System.ComponentModel.DataAnnotations;

namespace PurchaseManagement.API.Models
{
    public class Users
    {
        //public int Id { get; set; }
        //public string Username { get; set; }
        //public string PasswordHash { get; set; }
        //public string Role { get; set; }
        //public bool IsActive { get; set; }
        //public DateTime CreatedAt { get; set; }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "User"; // Default role

        public bool IsActive { get; set; } = true;

        // Navigation properties for audit trail (optional)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

    }
}
