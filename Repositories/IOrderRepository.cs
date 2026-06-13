using FoodHub.Models;

namespace FoodHub.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();

        Task<Order?> GetByIdAsync(int id);

        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);

        Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);

        Task<Order> CreateAsync(Order order);

        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
    }
}
