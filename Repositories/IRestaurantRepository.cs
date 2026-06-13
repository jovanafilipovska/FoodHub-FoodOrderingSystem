using FoodHub.Models;

namespace FoodHub.Repositories
{
    public interface IRestaurantRepository
    {
        Task<IEnumerable<Restaurant>> GetAllAsync();

        Task<Restaurant?> GetByIdAsync(int id);

        Task<Restaurant> CreateAsync(Restaurant restaurant);

        Task UpdateAsync(Restaurant restaurant);

        Task DeleteAsync(int id);
    }
}
