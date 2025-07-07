using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseManagement.API.DTOs;
using PurchaseManagement.API.Services.Interfaces;
using NLog;

namespace PurchaseManagement.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All actions require authentication
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly Logger _logger;

        public UsersController(IUserService userService)
        {
            _userService = userService;
            _logger = LogManager.GetCurrentClassLogger();
        }

  
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                _logger.Info("Getting all users");

                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting all users");
                return StatusCode(500, new { message = "An error occurred while fetching users" });
            }
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                _logger.Info($"Getting user by ID: {id}");

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.Warn($"User not found with ID: {id}");
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting user by ID: {id}");
                return StatusCode(500, new { message = "An error occurred while fetching user" });
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                _logger.Info($"Creating new user: {createUserDto.Username}");

                if (!ModelState.IsValid)
                {
                    _logger.Warn("Invalid create user request model state");
                    return BadRequest(ModelState);
                }

                var user = await _userService.CreateUserAsync(createUserDto);

                _logger.Info($"User created successfully: {user.Username}");
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warn(ex, $"Invalid operation during user creation: {createUserDto.Username}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error creating user: {createUserDto.Username}");
                return StatusCode(500, new { message = "An error occurred while creating user" });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                _logger.Info($"Updating user with ID: {id}");

                if (!ModelState.IsValid)
                {
                    _logger.Warn("Invalid update user request model state");
                    return BadRequest(ModelState);
                }

                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                if (user == null)
                {
                    _logger.Warn($"User not found for update with ID: {id}");
                    return NotFound(new { message = "User not found" });
                }

                _logger.Info($"User updated successfully: {user.Username}");
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warn(ex, $"Invalid operation during user update: {id}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error updating user with ID: {id}");
                return StatusCode(500, new { message = "An error occurred while updating user" });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                _logger.Info($"Deleting user with ID: {id}");

                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                {
                    _logger.Warn($"User not found for deletion with ID: {id}");
                    return NotFound(new { message = "User not found" });
                }

                _logger.Info($"User deleted successfully with ID: {id}");
                return Ok(new { message = "User deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warn(ex, $"Invalid operation during user deletion: {id}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error deleting user with ID: {id}");
                return StatusCode(500, new { message = "An error occurred while deleting user" });
            }
        }


        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleUserStatus(int id, [FromBody] ToggleUserStatusDto toggleStatusDto)
        {
            try
            {
                _logger.Info($"Toggling user status - ID: {id}, Active: {toggleStatusDto.IsActive}");

                var result = await _userService.ToggleUserStatusAsync(id, toggleStatusDto.IsActive);
                if (!result)
                {
                    _logger.Warn($"User not found for status toggle with ID: {id}");
                    return NotFound(new { message = "User not found" });
                }

                _logger.Info($"User status toggled successfully - ID: {id}, Active: {toggleStatusDto.IsActive}");
                return Ok(new { message = "User status updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.Warn(ex, $"Invalid operation during user status toggle: {id}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error toggling user status with ID: {id}");
                return StatusCode(500, new { message = "An error occurred while updating user status" });
            }
        }
    }
}