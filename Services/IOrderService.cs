using FoodHub.DTOs.Order;
using FoodHub.Models;

namespace FoodHub.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetAllAsync();

        Task<OrderDTO?> GetByIdAsync(int id);

        Task<IEnumerable<OrderDTO>> GetByUserAsync(string userId);

        Task<OrderDTO> CheckoutAsync(string userId);

        Task<OrderDTO?> UpdateStatusAsync(int orderId,OrderStatus status);

        Task<bool> CancelOrderAsync(int orderId,string userId);
    }
}
