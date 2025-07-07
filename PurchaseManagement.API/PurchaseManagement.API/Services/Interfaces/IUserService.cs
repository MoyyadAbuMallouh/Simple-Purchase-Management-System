using PurchaseManagement.API.DTOs;

namespace PurchaseManagement.API.Services.Interfaces
{
    public interface IUserService
    {
        //Task<IEnumerable<UserDto>> GetAllUsersAsync();
        //Task<UserDto?> GetUserByIdAsync(int id);
        //Task<UserDto?> GetUserByUsernameAsync(string username);
        //Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        //Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        //Task<bool> DeleteUserAsync(int id);
        //Task<UserDto?> AuthenticateAsync(string username, string password);
        //Task UpdateUserRoleAsync(int userId, UpdateUserRoleDto dto);


        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ToggleUserStatusAsync(int id, bool isActive);
    }
}
