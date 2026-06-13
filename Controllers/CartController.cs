using FoodHub.DTOs.Cart;
using FoodHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var cart =
                await _cartService.GetCartAsync(userId);

            if (cart == null)
                return NotFound();

            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart(
            [FromBody] AddToCartDTO dto)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var cart =
                await _cartService.AddToCartAsync(
                    userId,
                    dto);

            return Ok(cart);
        }

        [HttpPut("items/{menuItemId}")]
        public async Task<IActionResult> UpdateQuantity(
            int menuItemId,
            [FromBody] UpdateCartItemDTO dto)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var cart =
                await _cartService.UpdateQuantityAsync(
                    userId,
                    menuItemId,
                    dto.Quantity);

            if (cart == null)
                return NotFound();

            return Ok(cart);
        }

        [HttpDelete("items/{menuItemId}")]
        public async Task<IActionResult> RemoveItem(
            int menuItemId)
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var result =
                await _cartService.RemoveItemAsync(
                    userId,
                    menuItemId);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();

            if (userId == null)
                return Unauthorized();

            var result =
                await _cartService.ClearCartAsync(userId);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
