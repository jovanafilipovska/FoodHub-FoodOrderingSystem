using FoodHub.DTOs.Order;
using FoodHub.Models;
using FoodHub.Repositories;
using FoodHub.Services;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Xunit;

public class OrderServiceTests
{
    private readonly IOrderRepository _orderRepo;
    private readonly ICartRepository _cartRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _orderRepo = Substitute.For<IOrderRepository>();
        _cartRepo = Substitute.For<ICartRepository>();

        var store = Substitute.For<IUserStore<ApplicationUser>>();
        _userManager = Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);

        _sut = new OrderService(_orderRepo, _cartRepo, _userManager);
    }

    // =========================
    // GET BY ID
    // =========================

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsOrder()
    {
        _orderRepo.GetByIdAsync(1)
            .Returns(new Order { Id = 1 });

        var result = await _sut.GetByIdAsync(1);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNull()
    {
        _orderRepo.GetByIdAsync(1).Returns((Order?)null);

        var result = await _sut.GetByIdAsync(1);

        Assert.Null(result);
    }

    // =========================
    // CHECKOUT
    // =========================

    [Fact]
    public async Task CheckoutAsync_EmptyCart_ThrowsException()
    {
        _cartRepo.GetCartByUserIdAsync("u1")
            .Returns(new Cart { CartItems = new List<CartItem>() });

        await Assert.ThrowsAsync<Exception>(() =>
            _sut.CheckoutAsync("u1"));
    }

    [Fact]
    public async Task CheckoutAsync_HappyPath_CreatesOrder()
    {
        var cart = new Cart
        {
            Id = 1,
            CartItems = new List<CartItem>
            {
                new CartItem
                {
                    Quantity = 2,
                    MenuItem = new MenuItem { Price = 10 }
                }
            }
        };

        _cartRepo.GetCartByUserIdAsync("u1").Returns(cart);

        _orderRepo.CreateAsync(Arg.Any<Order>())
            .Returns(new Order { Id = 1 });

        _userManager.FindByIdAsync("u1")
            .Returns(new ApplicationUser { LoyaltyPoints = 0 });

        _userManager.UpdateAsync(Arg.Any<ApplicationUser>())
            .Returns(IdentityResult.Success);

        var result = await _sut.CheckoutAsync("u1");

        Assert.NotNull(result);
    }

    // =========================
    // UPDATE STATUS
    // =========================

    [Fact]
    public async Task UpdateStatusAsync_ValidOrder_UpdatesStatus()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Pending };

        _orderRepo.GetByIdAsync(1).Returns(order);

        _orderRepo.UpdateAsync(order).Returns(Task.CompletedTask);

        var result = await _sut.UpdateStatusAsync(1, OrderStatus.Delivered);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateStatusAsync_NotFound_ReturnsNull()
    {
        _orderRepo.GetByIdAsync(1).Returns((Order?)null);

        var result = await _sut.UpdateStatusAsync(1, OrderStatus.Delivered);

        Assert.Null(result);
    }

    // =========================
    // CANCEL ORDER
    // =========================

    [Fact]
    public async Task CancelOrderAsync_NotOwner_ReturnsFalse()
    {
        var order = new Order { Id = 1, CustomerId = "u2" };

        _orderRepo.GetByIdAsync(1).Returns(order);

        var result = await _sut.CancelOrderAsync(1, "u1");

        Assert.False(result);
    }

    [Fact]
    public async Task CancelOrderAsync_Delivered_ThrowsException()
    {
        var order = new Order
        {
            Id = 1,
            CustomerId = "u1",
            Status = OrderStatus.Delivered
        };

        _orderRepo.GetByIdAsync(1).Returns(order);

        await Assert.ThrowsAsync<Exception>(() =>
            _sut.CancelOrderAsync(1, "u1"));
    }

    [Fact]
    public async Task CancelOrderAsync_Happy_ReturnsTrue()
    {
        var order = new Order
        {
            Id = 1,
            CustomerId = "u1",
            Status = OrderStatus.Pending
        };

        _orderRepo.GetByIdAsync(1).Returns(order);

        _orderRepo.UpdateAsync(order).Returns(Task.CompletedTask);

        var result = await _sut.CancelOrderAsync(1, "u1");

        Assert.True(result);
    }
}