//using Microsoft.EntityFrameworkCore;
//using PurchaseManagement.API.Data;
//using PurchaseManagement.API.DTOs;
//using PurchaseManagement.API.Services.Interfaces;
//using System.Security.Cryptography;

////using PurchaseManagement.API.Models;
//using System.Text;

//namespace PurchaseManagement.API.Services
//{
//    public class UserService : IUserService
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IMappingService _mappingService;
//        private readonly ILogger<UserService> _logger;

//        public UserService(ApplicationDbContext context, IMappingService mappingService, ILogger<UserService> logger)
//        {
//            _context = context;
//            _mappingService = mappingService;
//            _logger = logger;
//        }

//        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
//        {
//            _logger.LogInformation("Service: Getting all users");

//            try
//            {
//                var users = await _context.Users.ToListAsync();
//                _logger.LogInformation("Service: Successfully retrieved {Count} users", users.Count);
//                return _mappingService.MapToUserDtos(users);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Service: Error retrieving all users");
//                throw;
//            }
//        }

//        public async Task<UserDto?> GetUserByIdAsync(int id)
//        {
//            _logger.LogInformation("Service: Getting user with ID: {UserId}", id);

//            try
//            {
//                var user = await _context.Users.FindAsync(id);
//                if (user == null)
//                {
//                    _logger.LogWarning("Service: User with ID {UserId} not found", id);
//                    return null;
//                }
//                _logger.LogInformation("Service: Successfully retrieved user {Username} (ID: {UserId})", user.Username, id);
//                return _mappingService.MapToUserDto(user);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Service: Error retrieving user with ID: {UserId}", id);
//                throw;
//            }
//        }

//        public async Task<UserDto?> GetUserByUsernameAsync(string username)
//        {
//            _logger.LogInformation("Service: Getting user with username: {Username}", username);

//            try
//            {
//                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
//                if (user == null)
//                {
//                    _logger.LogWarning("Service: User with username {Username} not found", username);
//                    return null;
//                }
//                _logger.LogInformation("Service: Successfully retrieved user {Username} (ID: {UserId})", user.Username, user.Id);
//                return _mappingService.MapToUserDto(user);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Service: Error retrieving user with username: {Username}", username);
//                throw;
//            }
//        }

//        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
//        {
//            _logger.LogInformation("Service: Creating new user: {Username}", createUserDto.Username);

//            try
//            {
//                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == createUserDto.Username);
//                if (existingUser != null)
//                {
//                    _logger.LogWarning("Service: Username {Username} already exists", createUserDto.Username);
//                    throw new InvalidOperationException($"Username '{createUserDto.Username}' already exists");
//                }

//                var user = _mappingService.MapToUser(createUserDto);
//                user.PasswordHash = await HashPasswordAsync(createUserDto.Password);
//                _context.Users.Add(user);
//                await _context.SaveChangesAsync();

//                _logger.LogInformation("Service: User created successfully with ID: {UserId}", user.Id);
//                return _mappingService.MapToUserDto(user);
//            }
//            catch (InvalidOperationException)
//            {
//                throw;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Service: Error creating user: {Username}", createUserDto.Username);
//                throw;
//            }
//        }

//        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
//        {
//            _logger.LogInformation("Service: Updating user with ID: {UserId}", id);

//            try
//            {
//                var user = await _context.Users.FindAsync(id);
//                if (user == null)
//                {
//                    _logger.LogWarning("Service: User with ID {UserId} not found", id);
//                    throw new InvalidOperationException($"User with ID {id} not found");
//                }

//                if (!string.IsNullOrEmpty(updateUserDto.Username) && updateUserDto.Username != user.Username)
//                {
//                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == updateUserDto.Username);
//                    if (existingUser != null)
//                    {
//                        _logger.LogWarning("Service: Username {Username} already exists", updateUserDto.Username);
//                        throw new InvalidOperationException($"Username '{updateUserDto.Username}' already exists");
//                    }
//                }

//                _mappingService.ApplyUpdate(user, updateUserDto);
//                if (!string.IsNullOrEmpty(updateUserDto.Password))
//                {
//                    user.PasswordHash = await HashPasswordAsync(updateUserDto.Password);
//                }

//                await _context.SaveChangesAsync();

//                _logger.LogInformation("Service: User updated successfully. ID: {UserId}", user.Id);
//                return _mappingService.MapToUserDto(user);
//            }
//            catch (InvalidOperationException)
//            {
//                throw;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Service: Error updating user with ID: {UserId}", id);
//                throw;
//            }
//        }

//        public async Task<bool> DeleteUserAsync(int id)
//        {
//            _logger.LogInformation("Service: Deleting user with ID: {UserId}", id);

//            try
//            {
//                var user = await _context.Users.FindAsync(id);
//                if (user == null)
//                {
//                    _logger.LogWarning("Service: User with ID {UserId} not found", id);
//                    return false;
//                }

//                _context.Users.Remove(user);
//                await _context.SaveChangesAsync();

//                _logger.LogInformation("Service: User deleted successfully. ID: {UserId}, Username: {Username}", id, user.Username);
//                return true;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Service: Error deleting user with ID: {UserId}", id);
//                throw;
//            }
//        }
//        public async Task<UserDto?> AuthenticateAsync(string username, string password)
//        {
//            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
//            if (user == null)
//            {
//                return null;
//            }
//            var passwordHash = await HashPasswordAsync(password); 
//            if (passwordHash != user.PasswordHash)
//            {
//                return null;
//            }
//            return _mappingService.MapToUserDto(user);
//        }

//        private Task<string> HashPasswordAsync(string password)
//        {
//            using (var sha256 = SHA256.Create())
//            {
//                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
//                var hash = Convert.ToBase64String(hashedBytes);
//                return Task.FromResult(hash); // Wrap in Task
//            }
//        }

//        public async Task UpdateUserRoleAsync(int userId, UpdateUserRoleDto dto)
//        {
//            var user = await _context.Users.FindAsync(userId);
//            if (user == null)
//                throw new KeyNotFoundException("User not found");

//            user.Role = dto.Role;
//            await _context.SaveChangesAsync();
//        }

//    }
//}

// Services/UserService.cs
using Microsoft.EntityFrameworkCore;
using PurchaseManagement.API.Data;
using PurchaseManagement.API.DTOs;
using PurchaseManagement.API.Services.Interfaces;
using NLog;
using PurchaseManagement.API.Services;
using PurchaseManagement.API.Models;

namespace PurchaseManagement.API.Services
{
    /// <summary>
    /// Service for managing user operations
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly Logger _logger;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
            _logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Gets all users from database
        /// </summary>
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            try
            {
                _logger.Info("Fetching all users");

                var users = await _context.Users
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Role = u.Role,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        LastLoginAt = u.LastLoginAt
                    })
                    .ToListAsync();

                _logger.Info($"Successfully fetched {users.Count} users");
                return users;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error fetching all users");
                throw;
            }
        }

        /// <summary>
        /// Gets user by ID
        /// </summary>
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            try
            {
                _logger.Info($"Fetching user with ID: {id}");

                var user = await _context.Users
                    .Where(u => u.Id == id)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Role = u.Role,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        LastLoginAt = u.LastLoginAt
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    _logger.Warn($"User not found with ID: {id}");
                    return null;
                }

                _logger.Info($"Successfully fetched user: {user.Username}");
                return user;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error fetching user with ID: {id}");
                throw;
            }
        }

        /// <summary>
        /// Creates new user
        /// </summary>
        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                _logger.Info($"Creating new user: {createUserDto.Username}");

                // Check if username already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == createUserDto.Username);

                if (existingUser != null)
                {
                    _logger.Warn($"Username already exists: {createUserDto.Username}");
                    throw new InvalidOperationException("Username already exists");
                }

                // Create new user
                var user = new Users
                {
                    Username = createUserDto.Username,
                    PasswordHash = AuthService.HashPassword(createUserDto.Password),
                    Role = createUserDto.Role,
                    IsActive = createUserDto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.Info($"Successfully created user: {user.Username} with ID: {user.Id}");

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
                _logger.Error(ex, $"Error creating user: {createUserDto.Username}");
                throw;
            }
        }

        /// <summary>
        /// Updates existing user
        /// </summary>
        public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            try
            {
                _logger.Info($"Updating user with ID: {id}");

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.Warn($"User not found for update with ID: {id}");
                    return null;
                }

                // Check if new username already exists (excluding current user)
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == updateUserDto.Username && u.Id != id);

                if (existingUser != null)
                {
                    _logger.Warn($"Username already exists: {updateUserDto.Username}");
                    throw new InvalidOperationException("Username already exists");
                }

                // Update user properties
                user.Username = updateUserDto.Username;
                user.Role = updateUserDto.Role;
                user.IsActive = updateUserDto.IsActive;

                await _context.SaveChangesAsync();

                _logger.Info($"Successfully updated user: {user.Username}");

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
                _logger.Error(ex, $"Error updating user with ID: {id}");
                throw;
            }
        }

        /// <summary>
        /// Deletes user by ID
        /// </summary>
        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                _logger.Info($"Deleting user with ID: {id}");

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.Warn($"User not found for deletion with ID: {id}");
                    return false;
                }

                // Don't allow deletion of the last admin user
                if (user.Role == "Admin")
                {
                    var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin" && u.IsActive);
                    if (adminCount <= 1)
                    {
                        _logger.Warn("Cannot delete the last admin user");
                        throw new InvalidOperationException("Cannot delete the last admin user");
                    }
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.Info($"Successfully deleted user: {user.Username}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error deleting user with ID: {id}");
                throw;
            }
        }

        /// <summary>
        /// Toggles user active status
        /// </summary>
        public async Task<bool> ToggleUserStatusAsync(int id, bool isActive)
        {
            try
            {
                _logger.Info($"Toggling user status - ID: {id}, Active: {isActive}");

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    _logger.Warn($"User not found for status toggle with ID: {id}");
                    return false;
                }

                // Don't allow deactivation of the last admin user
                if (user.Role == "Admin" && !isActive)
                {
                    var activeAdminCount = await _context.Users.CountAsync(u => u.Role == "Admin" && u.IsActive);
                    if (activeAdminCount <= 1)
                    {
                        _logger.Warn("Cannot deactivate the last admin user");
                        throw new InvalidOperationException("Cannot deactivate the last admin user");
                    }
                }

                user.IsActive = isActive;
                await _context.SaveChangesAsync();

                _logger.Info($"Successfully toggled user status: {user.Username} - Active: {isActive}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error toggling user status with ID: {id}");
                throw;
            }
        }
    }
}
