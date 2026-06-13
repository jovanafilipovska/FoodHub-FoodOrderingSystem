using AutoMapper;
using FoodHub.DTOs.Order;
using FoodHub.Models;
using FoodHub.Repositories;
using Microsoft.AspNetCore.Identity;

namespace FoodHub.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderDTO>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<OrderDTO?> GetByIdAsync(int id)
        {
            var order =
                await _orderRepository.GetByIdAsync(id);

            if (order == null)
                return null;

            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<IEnumerable<OrderDTO>>
            GetByUserAsync(string userId)
        {
            var orders =
                await _orderRepository.GetByUserIdAsync(userId);

            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<OrderDTO> CheckoutAsync(string userId)
        {
            var cart =
                await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
                throw new Exception("Cart is empty");

            var total =
                cart.CartItems.Sum(ci =>
                    ci.MenuItem.Price * ci.Quantity);

            var order = new Order
            {
                CustomerId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalPrice = total
            };

            foreach (var item in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    UnitPrice = item.MenuItem.Price
                });
            }

            await _orderRepository.CreateAsync(order);

            var user =
                await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                user.LoyaltyPoints += (int)(total / 10);

                await _userManager.UpdateAsync(user);
            }

            await _cartRepository.ClearCartAsync(cart.Id);

            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<OrderDTO?> UpdateStatusAsync(
            int orderId,
            OrderStatus status)
        {
            var order =
                await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                return null;

            order.Status = status;

            await _orderRepository.UpdateAsync(order);

            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<bool> CancelOrderAsync(
            int orderId,
            string userId)
        {
            var order =
                await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                return false;

            if (order.CustomerId != userId)
                return false;

            if (order.Status == OrderStatus.Delivered)
                throw new Exception(
                    "Delivered orders cannot be cancelled");

            if (order.Status == OrderStatus.Preparing)
                throw new Exception(
                    "Order is already being prepared");

            order.Status = OrderStatus.Cancelled;

            await _orderRepository.UpdateAsync(order);

            return true;
        }
    }
}
