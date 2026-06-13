using FoodHub.DTOs.Auth;

namespace FoodHub.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto);

        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto);
    }
}
