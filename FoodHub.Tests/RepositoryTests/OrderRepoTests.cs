using FoodHub.Data;
using FoodHub.Models;
using FoodHub.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FoodHub.Test.RepositoryTests
{
    public class OrderRepoTests : IDisposable
    {
        private readonly ApplicationDbContext _ctx;
        private readonly OrderRepository _sut;

        public OrderRepoTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"orders-{Guid.NewGuid()}")
                .Options;

            _ctx = new ApplicationDbContext(options);

            _ctx.Orders.AddRange(
                new Order { Id = 1, CustomerId = "u1", Status = OrderStatus.Pending },
                new Order { Id = 2, CustomerId = "u2", Status = OrderStatus.Delivered }
            );

            _ctx.SaveChanges();

            _sut = new OrderRepository(_ctx);
        }

        public void Dispose() => _ctx.Dispose();

        [Fact]
        public async Task GetAllAsync_ShouldReturnOrders()
        {
            var result = await _sut.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrderWithItems()
        {
            var result = await _sut.GetByIdAsync(1);

            Assert.NotNull(result!.OrderItems);
        }

        [Fact]
        public async Task GetByUserId_ShouldReturnUserOrders()
        {
            var result = await _sut.GetByUserIdAsync("u1");

            Assert.Single(result);
        }

        [Fact]
        public async Task GetByUserId_ShouldReturnEmpty_WhenNotFound()
        {
            var result = await _sut.GetByUserIdAsync("x");

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByStatus_ShouldReturnFilteredOrders()
        {
            var result = await _sut.GetByStatusAsync(OrderStatus.Delivered);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetByStatus_ShouldReturnEmpty_WhenNoMatch()
        {
            var result = await _sut.GetByStatusAsync(OrderStatus.Cancelled);

            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddOrder()
        {
            await _sut.CreateAsync(new Order
            {
                CustomerId = "u3",
                Status = OrderStatus.Pending
            });

            Assert.True(_ctx.Orders.Any(o => o.CustomerId == "u3"));
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveOrder()
        {
            await _sut.DeleteAsync(1);

            Assert.Null(await _sut.GetByIdAsync(1));
        }
    }
}