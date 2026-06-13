using AutoMapper;
using FoodHub.DTOs.MenuItem;
using FoodHub.Models;
using FoodHub.Repositories;

namespace FoodHub.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public MenuItemService(
            IMenuItemRepository menuItemRepository,
            IRestaurantRepository restaurantRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            _menuItemRepository = menuItemRepository;
            _restaurantRepository = restaurantRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MenuItemDTO>> GetAllAsync()
        {
            var menuItems = await _menuItemRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<MenuItemDTO>>(menuItems);
        }

        public async Task<MenuItemDTO?> GetByIdAsync(int id)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);

            if (menuItem == null)
                return null;

            return _mapper.Map<MenuItemDTO>(menuItem);
        }

        public async Task<IEnumerable<MenuItemDTO>>
            GetByRestaurantAsync(int restaurantId)
        {
            var items =
                await _menuItemRepository.GetByRestaurantAsync(
                    restaurantId);

            return _mapper.Map<IEnumerable<MenuItemDTO>>(items);
        }

        public async Task<MenuItemDTO> CreateAsync(
            CreateMenuItemDTO dto)
        {
            var restaurant =
                await _restaurantRepository.GetByIdAsync(
                    dto.RestaurantId);

            if (restaurant == null)
                throw new Exception("Restaurant not found");

            var category =
                await _categoryRepository.GetByIdAsync(
                    dto.CategoryId);

            if (category == null)
                throw new Exception("Category not found");

            if (dto.Price <= 0)
                throw new Exception(
                    "Price must be greater than zero");

            var menuItem = _mapper.Map<MenuItem>(dto);

            await _menuItemRepository.CreateAsync(menuItem);

            return _mapper.Map<MenuItemDTO>(menuItem);
        }

        public async Task<MenuItemDTO?> UpdateAsync(
            int id,
            UpdateMenuItemDTO dto)
        {
            var menuItem =
                await _menuItemRepository.GetByIdAsync(id);

            if (menuItem == null)
                return null;

            menuItem.Name = dto.Name;
            menuItem.Description = dto.Description;
            menuItem.ImageUrl = dto.ImageUrl;
            menuItem.Price = dto.Price;
            menuItem.IsAvailable = dto.IsAvailable;
            menuItem.CategoryId = dto.CategoryId;

            await _menuItemRepository.UpdateAsync(menuItem);

            return _mapper.Map<MenuItemDTO>(menuItem);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var menuItem =
                await _menuItemRepository.GetByIdAsync(id);

            if (menuItem == null)
                return false;

            await _menuItemRepository.DeleteAsync(id);

            return true;
        }
    }
}
