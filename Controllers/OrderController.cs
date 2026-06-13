using FoodHub.DTOs.Order;
using FoodHub.Models;
using FoodHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order =
                await _orderService.GetByIdAsync(id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var orders =
                await _orderService.GetByUserAsync(userId);

            return Ok(orders);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var order =
                await _orderService.CheckoutAsync(userId);

            return Ok(order);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] UpdateOrderStatusDTO dto)
        {
            if (!Enum.TryParse<OrderStatus>(
                dto.Status,
                true,
                out var status))
            {
                return BadRequest("Invalid status");
            }

            var order =
                await _orderService.UpdateStatusAsync(
                    id,
                    status);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [Authorize(Roles = "Customer")]
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var result =
                await _orderService.CancelOrderAsync(
                    id,
                    userId);

            if (!result)
                return NotFound();

            return Ok("Order cancelled successfully.");
        }
    }
}
