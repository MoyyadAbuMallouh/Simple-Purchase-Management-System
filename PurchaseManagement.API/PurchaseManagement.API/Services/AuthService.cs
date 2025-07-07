using Microsoft.IdentityModel.Tokens;
using PurchaseManagement.API.Data;
using PurchaseManagement.API.DTOs;
using PurchaseManagement.API.Models;
using PurchaseManagement.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NLog;


namespace PurchaseManagement.API.Services
{
    /// <summary>
    /// Service for handling authentication operations
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly Logger _logger;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Authenticates user and generates JWT token
        /// </summary>
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                _logger.Info($"Login attempt for username: {loginRequest.Username}");

                // Find user by username
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginRequest.Username && u.IsActive);

                if (user == null)
                {
                    _logger.Warn($"Login failed - User not found: {loginRequest.Username}");
                    return null;
                }

                // Verify password
                if (!VerifyPassword(loginRequest.Password, user.PasswordHash))
                {
                    _logger.Warn($"Login failed - Invalid password for user: {loginRequest.Username}");
                    return null;
                }

                // Update last login time
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = GenerateJwtToken(user);

                _logger.Info($"Login successful for user: {loginRequest.Username}");

                return new LoginResponseDto
                {
                    Token = token,
                    Username = user.Username,
                    Role = user.Role,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(20) // Token expiry
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error during login for username: {loginRequest.Username}");
                return null;
            }
        }

        /// <summary>
        /// Validates JWT token and extracts user info
        /// </summary>
        public async Task<UserDto?> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                var user = await _context.Users.FindAsync(userId);
                if (user == null || !user.IsActive)
                    return null;

                return new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating token");
                return null;
            }
        }

        /// <summary>
        /// Changes user password
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.Warn($"Password change failed - User not found: {userId}");
                    return false;
                }

                // Verify current password
                if (!VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
                {
                    _logger.Warn($"Password change failed - Invalid current password for user: {userId}");
                    return false;
                }

                // Hash new password
                user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
                await _context.SaveChangesAsync();

                _logger.Info($"Password changed successfully for user: {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error changing password for user: {userId}");
                return false;
            }
        }

        /// <summary>
        /// Generates JWT token for authenticated user
        /// </summary>
        private string GenerateJwtToken(Users user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("username", user.Username),
                    new Claim("role", user.Role),
                    new Claim(ClaimTypes.Role, user.Role) // Standard role claim
                }),
                Expires = DateTime.UtcNow.AddMinutes(20),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Hashes password using BCrypt
        /// </summary>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies password against hash
        /// </summary>
        private static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}