using FoodHub.Data;
using FoodHub.Models;
using FoodHub.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodHub.Test.RepositoryTests
{
    public class CategoryRepoTests : IDisposable
    {
        private readonly ApplicationDbContext _ctx;
        private readonly CategoryRepository _sut;

        public CategoryRepoTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"categories-{Guid.NewGuid()}")
                .Options;

            _ctx = new ApplicationDbContext(options);

            _ctx.Categories.AddRange(
                new Category { Id = 1, Name = "Pizza" },
                new Category { Id = 2, Name = "Burger" },
                new Category { Id = 3, Name = "Dessert" }
            );

            _ctx.SaveChanges();

            _sut = new CategoryRepository(_ctx);
        }

        public void Dispose() => _ctx.Dispose();

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCategories()
        {
            var result = (await _sut.GetAllAsync()).ToList();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectCategory()
        {
            var result = await _sut.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Pizza", result!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            var result = await _sut.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnCorrectCategory()
        {
            var result = await _sut.GetByNameAsync("Pizza");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnNull_WhenNotFound()
        {
            var result = await _sut.GetByNameAsync("Pasta");

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddCategory()
        {
            await _sut.CreateAsync(new Category { Name = "Drinks" });

            Assert.Equal(4, _ctx.Categories.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyCategory()
        {
            var cat = await _sut.GetByIdAsync(1);

            cat!.Name = "Updated";

            await _sut.UpdateAsync(cat);

            Assert.Equal("Updated", (await _sut.GetByIdAsync(1))!.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveCategory()
        {
            await _sut.DeleteAsync(1);

            Assert.Null(await _sut.GetByIdAsync(1));
        }
    }
}