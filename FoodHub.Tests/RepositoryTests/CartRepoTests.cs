using FoodHub.Data;
using FoodHub.Models;
using FoodHub.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodHub.Test.RepositoryTests
{
    public class CartRepoTests : IDisposable
    {
        private readonly ApplicationDbContext _ctx;
        private readonly CartRepository _sut;

        public CartRepoTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"cart-{Guid.NewGuid()}")
                .Options;

            _ctx = new ApplicationDbContext(options);

            _ctx.Carts.Add(new Cart
            {
                Id = 1,
                CustomerId = "u1",
                CartItems = new List<CartItem>
                {
                    new CartItem { Id = 1, MenuItemId = 1, Quantity = 2 }
                }
            });

            _ctx.SaveChanges();

            _sut = new CartRepository(_ctx);
        }

        public void Dispose() => _ctx.Dispose();

        [Fact]
        public async Task GetCartByUserId_ShouldReturnCart()
        {
            var result = await _sut.GetCartByUserIdAsync("u1");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetCartByUserId_ShouldReturnNull_WhenNotFound()
        {
            var result = await _sut.GetCartByUserIdAsync("x");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetCartItem_ShouldReturnNull_WhenNotFound()
        {
            var result = await _sut.GetCartItemAsync(999, 999);

            Assert.Null(result);
        }

        [Fact]
        public async Task ClearCart_ShouldRemoveAllItems()
        {
            await _sut.ClearCartAsync(1);

            Assert.Empty(_ctx.CartItems);
        }
    }
}