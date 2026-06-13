using FoodHub.DTOs.Restaurant;
using FoodHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantController(
            IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var restaurants =
                await _restaurantService.GetAllAsync();

            return Ok(restaurants);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var restaurant =
                await _restaurantService.GetByIdAsync(id);

            if (restaurant == null)
                return NotFound();

            return Ok(restaurant);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateRestaurantDTO dto)
        {
            var ownerId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ownerId == null)
                return Unauthorized();

            var restaurant =
                await _restaurantService.CreateAsync(
                    dto,
                    ownerId);

            return Ok(restaurant);
        }

        [Authorize(Roles = "Owner,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateRestaurantDTO dto)
        {
            var restaurant =
                await _restaurantService.UpdateAsync(id, dto);

            if (restaurant == null)
                return NotFound();

            return Ok(restaurant);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result =
                await _restaurantService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
