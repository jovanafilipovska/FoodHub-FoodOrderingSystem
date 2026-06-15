using FoodHub.DTOs.MenuItem;
using FoodHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemController(
            IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var menuItems =
                await _menuItemService.GetAllAsync();

            return Ok(menuItems);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var menuItem =
                await _menuItemService.GetByIdAsync(id);

            if (menuItem == null)
                return NotFound();

            return Ok(menuItem);
        }

        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetByRestaurant(
            int restaurantId)
        {
            var menuItems =
                await _menuItemService
                    .GetByRestaurantAsync(restaurantId);

            return Ok(menuItems);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateMenuItemDTO dto)
        {
            var menuItem =
                await _menuItemService.CreateAsync(dto);

            return Ok(menuItem);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateMenuItemDTO dto)
        {
            var menuItem =
                await _menuItemService.UpdateAsync(id, dto);

            if (menuItem == null)
                return NotFound();

            return Ok(menuItem);
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result =
                await _menuItemService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
