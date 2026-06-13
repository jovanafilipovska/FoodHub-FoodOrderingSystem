using FoodHub.Models;

namespace FoodHub.Repositories
{
    public interface IMenuItemRepository
    {
        Task<IEnumerable<MenuItem>> GetAllAsync();

        Task<MenuItem?> GetByIdAsync(int id);

        Task<IEnumerable<MenuItem>> GetByRestaurantAsync(int restaurantId);
        Task<IEnumerable<MenuItem>> GetAvailableItemsAsync();

        Task<MenuItem> CreateAsync(MenuItem menuItem);

        Task UpdateAsync(MenuItem menuItem);

        Task DeleteAsync(int id);
    }
}
