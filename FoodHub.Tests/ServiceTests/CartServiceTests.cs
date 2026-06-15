using FoodHub.DTOs.Cart;
using FoodHub.Models;
using FoodHub.Repositories;
using FoodHub.Services;
using NSubstitute;
using Xunit;

public class CartServiceTests
{
    private readonly ICartRepository _cartRepo;
    private readonly IMenuItemRepository _menuRepo;
    private readonly CartService _sut;

    public CartServiceTests()
    {
        _cartRepo = Substitute.For<ICartRepository>();
        _menuRepo = Substitute.For<IMenuItemRepository>();

        _sut = new CartService(_cartRepo, _menuRepo);
    }

    // =========================
    // GET CART
    // =========================

    [Fact]
    public async Task GetCartAsync_ExistingCart_ReturnsCartDTO()
    {
        var cart = new Cart
        {
            Id = 1,
            CustomerId = "u1",
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

        var result = await _sut.GetCartAsync("u1");

        Assert.NotNull(result);
        Assert.Equal(20, result!.TotalPrice);
    }

    [Fact]
    public async Task GetCartAsync_NoCart_ReturnsNull()
    {
        _cartRepo.GetCartByUserIdAsync("u1").Returns((Cart?)null);

        var result = await _sut.GetCartAsync("u1");

        Assert.Null(result);
    }

    // =========================
    // ADD TO CART
    // =========================

    [Fact]
    public async Task AddToCartAsync_NewCart_CreatesCartAndAddsItem()
    {
        _cartRepo.GetCartByUserIdAsync("u1").Returns((Cart?)null);

        _cartRepo.CreateCartAsync(Arg.Any<Cart>())
            .Returns(new Cart { Id = 1, CustomerId = "u1" });

        _menuRepo.GetByIdAsync(1)
            .Returns(new MenuItem { Id = 1, Price = 10 });

        _cartRepo.GetCartItemAsync(1, 1)
            .Returns((CartItem?)null);

        var result = await _sut.AddToCartAsync("u1",
            new AddToCartDTO { MenuItemId = 1, Quantity = 2 });

        Assert.NotNull(result);
        await _cartRepo.Received(1).CreateCartAsync(Arg.Any<Cart>());
    }

    [Fact]
    public async Task AddToCartAsync_MenuItemNotFound_ThrowsException()
    {
        _cartRepo.GetCartByUserIdAsync("u1")
            .Returns(new Cart { Id = 1, CustomerId = "u1", CartItems = new List<CartItem>() });

        _menuRepo.GetByIdAsync(1).Returns((MenuItem?)null);

        await Assert.ThrowsAsync<Exception>(() =>
            _sut.AddToCartAsync("u1",
                new AddToCartDTO { MenuItemId = 1, Quantity = 1 }));
    }

    // =========================
    // UPDATE QUANTITY
    // =========================

    [Fact]
    public async Task UpdateQuantityAsync_ExistingItem_UpdatesQuantity()
    {
        var cart = new Cart { Id = 1, CustomerId = "u1" };

        var item = new CartItem
        {
            CartId = 1,
            MenuItemId = 1,
            Quantity = 2
        };

        _cartRepo.GetCartByUserIdAsync("u1").Returns(cart);
        _cartRepo.GetCartItemAsync(1, 1).Returns(item);

        _cartRepo.UpdateCartItemAsync(item).Returns(Task.CompletedTask);

        var result = await _sut.UpdateQuantityAsync("u1", 1, 5);

        Assert.NotNull(result);
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public async Task UpdateQuantityAsync_NoCart_ReturnsNull()
    {
        _cartRepo.GetCartByUserIdAsync("u1").Returns((Cart?)null);

        var result = await _sut.UpdateQuantityAsync("u1", 1, 2);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateQuantityAsync_ItemNotFound_ReturnsNull()
    {
        _cartRepo.GetCartByUserIdAsync("u1")
            .Returns(new Cart { Id = 1 });

        _cartRepo.GetCartItemAsync(1, 1).Returns((CartItem?)null);

        var result = await _sut.UpdateQuantityAsync("u1", 1, 2);

        Assert.Null(result);
    }

    // =========================
    // REMOVE ITEM
    // =========================

    [Fact]
    public async Task RemoveItemAsync_ItemExists_ReturnsTrue()
    {
        var cart = new Cart { Id = 1, CustomerId = "u1" };

        var item = new CartItem { Id = 10, CartId = 1, MenuItemId = 1 };

        _cartRepo.GetCartByUserIdAsync("u1").Returns(cart);
        _cartRepo.GetCartItemAsync(1, 1).Returns(item);

        _cartRepo.RemoveCartItemAsync(10).Returns(Task.CompletedTask);

        var result = await _sut.RemoveItemAsync("u1", 1);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveItemAsync_NoCart_ReturnsFalse()
    {
        _cartRepo.GetCartByUserIdAsync("u1").Returns((Cart?)null);

        var result = await _sut.RemoveItemAsync("u1", 1);

        Assert.False(result);
    }

    // =========================
    // CLEAR CART
    // =========================

    [Fact]
    public async Task ClearCartAsync_ExistingCart_ReturnsTrue()
    {
        _cartRepo.GetCartByUserIdAsync("u1")
            .Returns(new Cart { Id = 1 });

        _cartRepo.ClearCartAsync(1).Returns(Task.CompletedTask);

        var result = await _sut.ClearCartAsync("u1");

        Assert.True(result);
    }

    [Fact]
    public async Task ClearCartAsync_NoCart_ReturnsFalse()
    {
        _cartRepo.GetCartByUserIdAsync("u1").Returns((Cart?)null);

        var result = await _sut.ClearCartAsync("u1");

        Assert.False(result);
    }
}