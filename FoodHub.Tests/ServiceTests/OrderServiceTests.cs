using AutoMapper;
using FoodHub.DTOs.Order;
using FoodHub.Models;
using FoodHub.Profiles;
using FoodHub.Repositories;
using FoodHub.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

public class OrderServiceTests
{
    // ── shared infrastructure ────────────────────────────────────────────────
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly Mock<ICartRepository> _cartRepoMock = new();
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly IMapper _mapper;
    private readonly OrderService _sut;

    // Shared test data
    private readonly MenuItem _menuItem = new()
    {
        Id = 1,
        Name = "Margherita Pizza",
        Price = 10.00m,
        IsAvailable = true,
        RestaurantId = 1,
        CategoryId = 1
    };

    private readonly ApplicationUser _customer = new()
    {
        Id = "cust-1",
        UserName = "customer@test.com",
        Email = "customer@test.com",
        FirstName = "Alice",
        LastName = "Smith",
        LoyaltyPoints = 0
    };

    public OrderServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = mapperConfig.CreateMapper();

        // UserManager requires a store; mock it via its protected constructor signature
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _sut = new OrderService(
            _orderRepoMock.Object,
            _cartRepoMock.Object,
            _userManagerMock.Object,
            _mapper);
    }

    // ── helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Build a complete Order whose OrderItems have MenuItem nav props set.
    /// Without MenuItem populated, AutoMapper's OrderItemDTO.MenuItemName mapping
    /// fails silently and returns null, causing Assert.NotNull to fail.
    /// </summary>
    private Order MakeOrder(int id = 1, OrderStatus status = OrderStatus.Pending)
    {
        var orderItem = new OrderItem
        {
            Id = 1,
            OrderId = id,
            MenuItemId = _menuItem.Id,
            Quantity = 2,
            UnitPrice = _menuItem.Price,
            MenuItem = _menuItem        // ← nav prop must be set for AutoMapper
        };

        return new Order
        {
            Id = id,
            CustomerId = _customer.Id,
            Customer = _customer,
            OrderDate = DateTime.UtcNow,
            TotalPrice = 20.00m,
            Status = status,
            OrderItems = new List<OrderItem> { orderItem }
        };
    }

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsOrder()
    {
        var order = MakeOrder(id: 1);

        // The repository must be set up to return the order for id=1.
        // The original tests returned null here because the setup used a
        // mismatched id or wasn't called at all.
        _orderRepoMock.Setup(r => r.GetByIdAsync(1))
                      .ReturnsAsync(order);

        var result = await _sut.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(20.00m, result.TotalPrice);
        Assert.Equal("Pending", result.Status);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsNull()
    {
        _orderRepoMock.Setup(r => r.GetByIdAsync(999))
                      .ReturnsAsync((Order?)null);

        var result = await _sut.GetByIdAsync(999);

        Assert.Null(result);
    }

    // ── GetAllAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllOrders()
    {
        var orders = new List<Order> { MakeOrder(1), MakeOrder(2) };

        _orderRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(orders);

        var result = await _sut.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    // ── GetByUserAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByUserAsync_ReturnsUserOrders()
    {
        var orders = new List<Order> { MakeOrder(1), MakeOrder(3) };

        _orderRepoMock.Setup(r => r.GetByUserIdAsync(_customer.Id))
                      .ReturnsAsync(orders);

        var result = await _sut.GetByUserAsync(_customer.Id);

        Assert.Equal(2, result.Count());
    }

    // ── CheckoutAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task CheckoutAsync_HappyPath_CreatesOrder()
    {
        // Cart with one item whose MenuItem nav prop is populated
        var cartItem = new CartItem
        {
            Id = 1,
            CartId = 1,
            MenuItemId = _menuItem.Id,
            Quantity = 2,
            MenuItem = _menuItem    // ← needed for total calculation AND AutoMapper
        };
        var cart = new Cart
        {
            Id = 1,
            CustomerId = _customer.Id,
            CartItems = new List<CartItem> { cartItem }
        };

        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(_customer.Id))
                     .ReturnsAsync(cart);

        // UserManager must find the customer so loyalty-points update succeeds
        _userManagerMock.Setup(um => um.FindByIdAsync(_customer.Id))
                        .ReturnsAsync(_customer);
        _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<ApplicationUser>()))
                        .ReturnsAsync(IdentityResult.Success);

        // CreateAsync stores the order (no return value needed; order.Id stays 0
        // in-memory but DTO will still be mapped)
        _orderRepoMock.Setup(r => r.CreateAsync(It.IsAny<Order>()))
              .ReturnsAsync((Order o) => o);

        _cartRepoMock.Setup(r => r.ClearCartAsync(cart.Id))
                     .Returns(Task.CompletedTask);

        var result = await _sut.CheckoutAsync(_customer.Id);

        Assert.NotNull(result);
        Assert.Equal(20.00m, result.TotalPrice);   // 10.00 × 2
        Assert.Equal("Pending", result.Status);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task CheckoutAsync_EmptyCart_ThrowsException()
    {
        var emptyCart = new Cart { Id = 1, CustomerId = _customer.Id, CartItems = new List<CartItem>() };

        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(_customer.Id))
                     .ReturnsAsync(emptyCart);

        await Assert.ThrowsAsync<Exception>(() => _sut.CheckoutAsync(_customer.Id));
    }

    [Fact]
    public async Task CheckoutAsync_NullCart_ThrowsException()
    {
        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(_customer.Id))
                     .ReturnsAsync((Cart?)null);

        await Assert.ThrowsAsync<Exception>(() => _sut.CheckoutAsync(_customer.Id));
    }

    // ── UpdateStatusAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateStatusAsync_ValidOrder_UpdatesStatus()
    {
        var order = MakeOrder(id: 1, status: OrderStatus.Pending);

        // Must return the order – the original test failed because the mock
        // wasn't set up (returned null by default), so _mapper.Map crashed.
        _orderRepoMock.Setup(r => r.GetByIdAsync(1))
                      .ReturnsAsync(order);

        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>()))
                      .Returns(Task.CompletedTask);

        var result = await _sut.UpdateStatusAsync(1, OrderStatus.Preparing);

        Assert.NotNull(result);
        Assert.Equal("Preparing", result.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_OrderNotFound_ReturnsNull()
    {
        _orderRepoMock.Setup(r => r.GetByIdAsync(999))
                      .ReturnsAsync((Order?)null);

        var result = await _sut.UpdateStatusAsync(999, OrderStatus.Delivered);

        Assert.Null(result);
    }

    // ── CancelOrderAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task CancelOrderAsync_PendingOrder_ReturnsTrue()
    {
        var order = MakeOrder(id: 1, status: OrderStatus.Pending);
        _orderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);
        _orderRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);

        var result = await _sut.CancelOrderAsync(1, _customer.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task CancelOrderAsync_WrongUser_ReturnsFalse()
    {
        var order = MakeOrder(id: 1, status: OrderStatus.Pending);
        _orderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

        var result = await _sut.CancelOrderAsync(1, "wrong-user");

        Assert.False(result);
    }

    [Fact]
    public async Task CancelOrderAsync_DeliveredOrder_ThrowsException()
    {
        var order = MakeOrder(id: 1, status: OrderStatus.Delivered);
        _orderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

        await Assert.ThrowsAsync<Exception>(() => _sut.CancelOrderAsync(1, _customer.Id));
    }

    [Fact]
    public async Task CancelOrderAsync_PreparingOrder_ThrowsException()
    {
        var order = MakeOrder(id: 1, status: OrderStatus.Preparing);
        _orderRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(order);

        await Assert.ThrowsAsync<Exception>(() => _sut.CancelOrderAsync(1, _customer.Id));
    }
}