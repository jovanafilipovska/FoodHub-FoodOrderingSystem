using FoodHub.DTOs.Restaurant;

namespace FoodHub.Services
{
    public interface IRestaurantService
    {
        Task<IEnumerable<RestaurantDTO>> GetAllAsync();

        Task<RestaurantDTO?> GetByIdAsync(int id);

        Task<RestaurantDTO> CreateAsync(CreateRestaurantDTO dto,string ownerId);

        Task<RestaurantDTO?> UpdateAsync(int id,UpdateRestaurantDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}
