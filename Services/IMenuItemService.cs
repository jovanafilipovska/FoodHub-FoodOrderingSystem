using FoodHub.DTOs.MenuItem;

namespace FoodHub.Services
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItemDTO>> GetAllAsync();

        Task<MenuItemDTO?> GetByIdAsync(int id);

        Task<IEnumerable<MenuItemDTO>> GetByRestaurantAsync(int restaurantId);

        Task<MenuItemDTO> CreateAsync(CreateMenuItemDTO dto);

        Task<MenuItemDTO?> UpdateAsync(int id, UpdateMenuItemDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}
