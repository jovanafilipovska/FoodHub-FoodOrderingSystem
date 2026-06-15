using FoodHub.Data;
using FoodHub.Models;
using FoodHub.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodHub.Test.RepositoryTests
{
    public class MenuItemRepoTests : IDisposable
    {
        private readonly ApplicationDbContext _ctx;
        private readonly MenuItemRepository _sut;

        public MenuItemRepoTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"menu-{Guid.NewGuid()}")
                .Options;

            _ctx = new ApplicationDbContext(options);

            _ctx.MenuItems.AddRange(
                new MenuItem { Id = 1, Name = "Pizza", RestaurantId = 1, CategoryId = 1, IsAvailable = true },
                new MenuItem { Id = 2, Name = "Burger", RestaurantId = 1, CategoryId = 2, IsAvailable = true },
                new MenuItem { Id = 3, Name = "Ice Cream", RestaurantId = 2, CategoryId = 3, IsAvailable = false }
            );

            _ctx.SaveChanges();

            _sut = new MenuItemRepository(_ctx);
        }

        public void Dispose() => _ctx.Dispose();

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllItems()
        {
            var result = await _sut.GetAllAsync();

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnItemWithIncludes()
        {
            var result = await _sut.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.NotNull(result!.Restaurant);
            Assert.NotNull(result.Category);
        }

        [Fact]
        public async Task GetByRestaurantAsync_ShouldReturnCorrectItems()
        {
            var result = await _sut.GetByRestaurantAsync(1);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAvailableItemsAsync_ShouldReturnOnlyAvailable()
        {
            var result = await _sut.GetAvailableItemsAsync();

            Assert.All(result, m => Assert.True(m.IsAvailable));
        }

        [Fact]
        public async Task GetAvailableItemsAsync_ShouldNotReturnUnavailableItems()
        {
            var result = await _sut.GetAvailableItemsAsync();

            Assert.DoesNotContain(result, m => !m.IsAvailable);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddItem()
        {
            await _sut.CreateAsync(new MenuItem
            {
                Name = "Salad",
                RestaurantId = 1,
                CategoryId = 1,
                IsAvailable = true
            });

            Assert.Equal(4, _ctx.MenuItems.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyItem()
        {
            var item = await _sut.GetByIdAsync(1);

            item!.Name = "Updated";

            await _sut.UpdateAsync(item);

            Assert.Equal("Updated", (await _sut.GetByIdAsync(1))!.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveItem()
        {
            await _sut.DeleteAsync(1);

            Assert.Null(await _sut.GetByIdAsync(1));
        }
    }
}