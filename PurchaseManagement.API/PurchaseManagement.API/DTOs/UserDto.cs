using System.ComponentModel.DataAnnotations;

namespace PurchaseManagement.API.DTOs
{
    //public class CreateUserDto
    //{
    //    [Required(ErrorMessage = "Username is required")]
    //    [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    //    public string Username { get; set; } = string.Empty;

    //    [Required(ErrorMessage = "Password is required")]
    //    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    //    public string Password { get; set; } = string.Empty;

    //    //[Required(ErrorMessage = "Role is required")]
    //    //[StringLength(20, ErrorMessage = "Role cannot exceed 20 characters")]
    //    //public string Role { get; set; } = string.Empty;

    //    [Required(ErrorMessage = "IsActive is required")]
    //    public bool IsActive { get; set; }
    //}

    //// DTO for updating an existing user
    //public class UpdateUserDto
    //{
    //    [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    //    public string? Username { get; set; }

    //    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    //    public string? Password { get; set; }

    //    [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters")]
    //    public string? Role { get; set; }

    //    public bool? IsActive { get; set; }
    //}

    //// DTO for returning user data
    //public class UserDto
    //{
    //    public int Id { get; set; }
    //    public string Username { get; set; } = string.Empty;
    //    public string Role { get; set; } = string.Empty;
    //    public bool IsActive { get; set; }
    //    public DateTime CreatedAt { get; set; }
    //}

    //public class UpdateUserRoleDto
    //{
    //    [Required]
    //    public string Role { get; set; } = string.Empty;
    //}

    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for successful login response
    /// </summary>
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// DTO for user registration (Admin only)
    /// </summary>
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for user display (without sensitive data)
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    /// <summary>
    /// DTO for updating user information
    /// </summary>
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for changing password
    /// </summary>
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }


    public class ToggleUserStatusDto
    {
        [Required]
        public bool IsActive { get; set; }
    }
}
