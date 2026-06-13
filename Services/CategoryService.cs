using AutoMapper;
using FoodHub.DTOs.Category;
using FoodHub.Models;
using FoodHub.Repositories;

namespace FoodHub.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return null;

            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO> CreateAsync(CreateCategoryDTO dto)
        {
            var existingCategory =
                await _categoryRepository.GetByNameAsync(dto.Name);

            if (existingCategory != null)
                throw new Exception("Category already exists");

            var category = _mapper.Map<Category>(dto);

            await _categoryRepository.CreateAsync(category);

            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO?> UpdateAsync(
            int id,
            UpdateCategoryDTO dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return null;

            category.Name = dto.Name;

            await _categoryRepository.UpdateAsync(category);

            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return false;

            await _categoryRepository.DeleteAsync(id);

            return true;
        }
    }
}
