//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using PurchaseManagement.API.DTOs;
//using PurchaseManagement.API.Services.Interfaces;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace PurchaseManagement.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AuthController : ControllerBase
//    {
//        private readonly IUserService _userService;
//        private readonly IConfiguration _configuration;

//        public AuthController(IUserService userService, IConfiguration configuration)
//        {
//            _userService = userService;
//            _configuration = configuration;
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
//        {
//            var userDto = await _userService.AuthenticateAsync(loginDto.Username, loginDto.Password);
//            if (userDto == null)
//            {
//                return Unauthorized("Invalid username or password.");
//            }

//            var token = GenerateJwtToken(userDto);
//            return Ok(new { Token = token });
//        }

//        [HttpPost("register")]
//        public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
//        {
//            try
//            {
//                var userDto = await _userService.CreateUserAsync(createUserDto);
//                return CreatedAtAction(nameof(GetUser), new { id = userDto.Id }, userDto);
//            }
//            catch (InvalidOperationException ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }

//        // Helper method to generate JWT token
//        private string GenerateJwtToken(UserDto user)
//        {
//            var claims = new[]
//            {
//            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//            new Claim(ClaimTypes.Name, user.Username),
//            new Claim(ClaimTypes.Role, user.Role)
//        };

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
//            var token = new JwtSecurityToken(
//                issuer: _configuration["Jwt:Issuer"],
//                audience: _configuration["Jwt:Audience"],
//                claims: claims,
//                expires: DateTime.Now.AddHours(1),
//                signingCredentials: creds);

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }

//        // For demonstration; typically, this would be in a UsersController with [Authorize(Roles = "Admin")]
//        [HttpGet("users/{id}")]
//        public async Task<IActionResult> GetUser(int id)
//        {
//            var user = await _userService.GetUserByIdAsync(id);
//            if (user == null)
//            {
//                return NotFound();
//            }
//            return Ok(user);
//        }
//    }

//    public class LoginDto
//    {
//        public string Username { get; set; } = string.Empty;
//        public string Password { get; set; } = string.Empty;
//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using NLog;
using PurchaseManagement.API.DTOs;
using PurchaseManagement.API.Services.Interfaces;

namespace PurchaseManagement.API.Controllers
{
    /// <summary>
    /// Controller for authentication operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly Logger _logger;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
            _logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Authenticates user and returns JWT token
        /// </summary>
        /// <param name="loginRequest">Login credentials</param>
        /// <returns>JWT token with user information</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                _logger.Info($"Login request received for: {loginRequest.Username}");

                if (!ModelState.IsValid)
                {
                    _logger.Warn("Invalid login request model state");
                    return BadRequest(ModelState);
                }

                var result = await _authService.LoginAsync(loginRequest);

                if (result == null)
                {
                    _logger.Warn($"Login failed for user: {loginRequest.Username}");
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                _logger.Info($"Login successful for user: {result.Username}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error during login for user: {loginRequest.Username}");
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Changes current user's password
        /// </summary>
        /// <param name="changePasswordDto">Password change request</param>
        /// <returns>Success status</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                // Get current user ID from JWT token
                var userIdClaim = User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    _logger.Warn("Invalid user ID in token");
                    return Unauthorized();
                }

                _logger.Info($"Password change request for user ID: {userId}");

                if (!ModelState.IsValid)
                {
                    _logger.Warn("Invalid password change request model state");
                    return BadRequest(ModelState);
                }

                var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);

                if (!result)
                {
                    _logger.Warn($"Password change failed for user ID: {userId}");
                    return BadRequest(new { message = "Current password is incorrect" });
                }

                _logger.Info($"Password changed successfully for user ID: {userId}");
                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during password change");
                return StatusCode(500, new { message = "An error occurred during password change" });
            }
        }

        /// <summary>
        /// Gets current user information from JWT token
        /// </summary>
        /// <returns>Current user information</returns>
        //[HttpGet("me")]
        //[Authorize]
        //public async Task<IActionResult> GetCurrentUser()
        //{
        //    try
        //    {
        //        var userIdClaim = User.FindFirst("id")?.Value;
        //        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        //        {
        //            _logger.Warn("Invalid user ID in token");
        //            return Unauthorized();
        //        }

        //        // Extract user info from token claims
        //        var userInfo = new
        //        {
        //            Id = userId,
        //            Username = User.FindFirst("username")?.Value,
        //            Role = User.FindFirst("role")?.Value
        //        };

        //        return Ok(userInfo);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error(ex, "Error getting current user");
        //        return StatusCode(500, new { message = "An error occurred getting user information" });
        //    }
        //}

        /// <summary>
        /// Logs out user (client-side token removal)
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // JWT tokens are stateless, so logout is handled on client side
            // This endpoint is for consistency and logging purposes
            var username = User.FindFirst("username")?.Value;
            _logger.Info($"User logged out: {username}");

            return Ok(new { message = "Logged out successfully" });
        }
    }
}