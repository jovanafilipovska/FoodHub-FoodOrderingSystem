using FoodHub.DTOs.Cart;

namespace FoodHub.Services
{
    public interface ICartService
    {
        Task<CartDTO?> GetCartAsync(string userId);

        Task<CartDTO> AddToCartAsync(
            string userId,
            AddToCartDTO dto);

        Task<CartDTO?> UpdateQuantityAsync(string userId,int menuItemId,int quantity);

        Task<bool> RemoveItemAsync(string userId,int menuItemId);

        Task<bool> ClearCartAsync(string userId);
    }
}
