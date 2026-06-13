using FoodHub.Models;

namespace FoodHub.Services
{
    public interface ITokenService
    {
        Task<string> CreateToken(ApplicationUser user);
    }
}
