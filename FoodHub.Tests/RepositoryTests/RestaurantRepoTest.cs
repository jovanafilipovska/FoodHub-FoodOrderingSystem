using FoodHub.Data;
using FoodHub.Models;
using FoodHub.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodHub.Test.RepositoryTests
{
    public class RestaurantRepoTests : IDisposable
    {
        private readonly ApplicationDbContext _ctx;
        private readonly RestaurantRepository _sut;

        public RestaurantRepoTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"rest-{Guid.NewGuid()}")
                .Options;

            _ctx = new ApplicationDbContext(options);

            _ctx.Restaurants.AddRange(
                new Restaurant { Id = 1, Name = "R1", OwnerId = "u1" },
                new Restaurant { Id = 2, Name = "R2", OwnerId = "u2" }
            );

            _ctx.SaveChanges();

            _sut = new RestaurantRepository(_ctx);
        }

        public void Dispose() => _ctx.Dispose();

        [Fact]
        public async Task GetAllAsync_ShouldReturnRestaurants()
        {
            var result = await _sut.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldIncludeOwnerAndMenuItems()
        {
            var result = await _sut.GetByIdAsync(1);

            Assert.NotNull(result!.Owner);
            Assert.NotNull(result.MenuItems);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            var result = await _sut.GetByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddRestaurant()
        {
            await _sut.CreateAsync(new Restaurant { Name = "R3", OwnerId = "u3" });

            Assert.Equal(3, _ctx.Restaurants.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyRestaurant()
        {
            var r = await _sut.GetByIdAsync(1);

            r!.Name = "Updated";

            await _sut.UpdateAsync(r);

            Assert.Equal("Updated", (await _sut.GetByIdAsync(1))!.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveRestaurant()
        {
            await _sut.DeleteAsync(1);

            Assert.Null(await _sut.GetByIdAsync(1));
        }
    }
}