using AutoMapper;
using FoodHub.DTOs.Restaurant;
using FoodHub.Models;
using FoodHub.Repositories;

namespace FoodHub.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMapper _mapper;

        public RestaurantService(
            IRestaurantRepository restaurantRepository,
            IMapper mapper)
        {
            _restaurantRepository = restaurantRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RestaurantDTO>> GetAllAsync()
        {
            var restaurants = await _restaurantRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<RestaurantDTO>>(restaurants);
        }

        public async Task<RestaurantDTO?> GetByIdAsync(int id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
                return null;

            return _mapper.Map<RestaurantDTO>(restaurant);
        }

        public async Task<RestaurantDTO> CreateAsync(
            CreateRestaurantDTO dto,
            string ownerId)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);

            restaurant.OwnerId = ownerId;

            await _restaurantRepository.CreateAsync(restaurant);

            return _mapper.Map<RestaurantDTO>(restaurant);
        }

        public async Task<RestaurantDTO?> UpdateAsync(
            int id,
            UpdateRestaurantDTO dto)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
                return null;

            restaurant.Name = dto.Name;
            restaurant.ImageUrl = dto.ImageUrl;
            restaurant.Address = dto.Address;
            restaurant.PhoneNumber = dto.PhoneNumber;
            restaurant.Description = dto.Description;

            await _restaurantRepository.UpdateAsync(restaurant);

            return _mapper.Map<RestaurantDTO>(restaurant);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(id);

            if (restaurant == null)
                return false;

            await _restaurantRepository.DeleteAsync(id);

            return true;
        }
    }
}
