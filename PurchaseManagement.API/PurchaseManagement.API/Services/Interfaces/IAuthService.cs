using PurchaseManagement.API.DTOs;

namespace PurchaseManagement.API.Services.Interfaces
{
    public interface IAuthService
    {

        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
        Task<UserDto?> ValidateTokenAsync(string token);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    }
}
