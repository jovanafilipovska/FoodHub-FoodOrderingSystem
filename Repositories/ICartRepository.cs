using FoodHub.Models;

namespace FoodHub.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(string userId);

        Task<CartItem?> GetCartItemAsync(int cartId, int menuItemId);
        Task<Cart> CreateCartAsync(Cart cart);

        Task AddItemAsync(CartItem cartItem);

        Task UpdateCartItemAsync(CartItem cartItem);

        Task RemoveCartItemAsync(int cartItemId);

        Task ClearCartAsync(int cartId);
    }
}
