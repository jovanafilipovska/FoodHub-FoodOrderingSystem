using FoodHub.DTOs.Auth;
using FoodHub.Models;
using Microsoft.AspNetCore.Identity;

namespace FoodHub.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDTO> RegisterAsync(
            RegisterDTO registerDto)
        {
            var existingUser =
                await _userManager.FindByEmailAsync(
                    registerDto.Email);

            if (existingUser != null)
                throw new Exception(
                    "User already exists");

            var user = new ApplicationUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                LoyaltyPoints = 0
            };

            var result =
                await _userManager.CreateAsync(
                    user,
                    registerDto.Password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(", ",
                    result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(
                user,
                registerDto.Role);

            var token =
                await _tokenService.CreateToken(user);

            return new AuthResponseDTO
            {
                Token = token
            };
        }

        public async Task<AuthResponseDTO> LoginAsync(
            LoginDTO loginDto)
        {
            var user =
                await _userManager.FindByEmailAsync(
                    loginDto.Email);

            if (user == null)
                throw new Exception(
                    "Invalid email or password");

            var passwordValid =
                await _userManager.CheckPasswordAsync(
                    user,
                    loginDto.Password);

            if (!passwordValid)
                throw new Exception(
                    "Invalid email or password");

            var roles =
                await _userManager.GetRolesAsync(user);

            var token =
                await _tokenService.CreateToken(user);

            return new AuthResponseDTO
            {
                Token = token
            };
        }
    }
}
