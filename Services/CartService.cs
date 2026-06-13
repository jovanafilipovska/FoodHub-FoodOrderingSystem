using AutoMapper;
using FoodHub.DTOs.Cart;
using FoodHub.Models;
using FoodHub.Repositories;

namespace FoodHub.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IMapper _mapper;

        public CartService(
            ICartRepository cartRepository,
            IMenuItemRepository menuItemRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _menuItemRepository = menuItemRepository;
            _mapper = mapper;
        }

        public async Task<CartDTO?> GetCartAsync(string userId)
        {
            var cart = await _cartRepository
                .GetCartByUserIdAsync(userId);

            if (cart == null)
                return null;

            var dto = _mapper.Map<CartDTO>(cart);

            dto.TotalPrice = cart.CartItems.Sum(ci =>
                ci.MenuItem.Price * ci.Quantity);

            return dto;
        }

        public async Task<CartDTO> AddToCartAsync(
            string userId,
            AddToCartDTO dto)
        {
            var cart =
                await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = userId
                };

                cart =
                    await _cartRepository.CreateCartAsync(cart);
            }

            var menuItem =
                await _menuItemRepository.GetByIdAsync(
                    dto.MenuItemId);

            if (menuItem == null)
                throw new Exception("Menu item not found");

            var existingItem =
                await _cartRepository.GetCartItemAsync(
                    cart.Id,
                    dto.MenuItemId);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;

                await _cartRepository
                    .UpdateCartItemAsync(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    MenuItemId = dto.MenuItemId,
                    Quantity = dto.Quantity
                };

                await _cartRepository.AddItemAsync(cartItem);
            }

            return await GetCartAsync(userId)
                ?? throw new Exception("Cart error");
        }

        public async Task<CartDTO?> UpdateQuantityAsync(
            string userId,
            int menuItemId,
            int quantity)
        {
            var cart =
                await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
                return null;

            var item =
                await _cartRepository.GetCartItemAsync(
                    cart.Id,
                    menuItemId);

            if (item == null)
                return null;

            item.Quantity = quantity;

            await _cartRepository.UpdateCartItemAsync(item);

            return await GetCartAsync(userId);
        }

        public async Task<bool> RemoveItemAsync(
            string userId,
            int menuItemId)
        {
            var cart =
                await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
                return false;

            var item =
                await _cartRepository.GetCartItemAsync(
                    cart.Id,
                    menuItemId);

            if (item == null)
                return false;

            await _cartRepository.RemoveCartItemAsync(item.Id);

            return true;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var cart =
                await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
                return false;

            await _cartRepository.ClearCartAsync(cart.Id);

            return true;
        }
    }
}
