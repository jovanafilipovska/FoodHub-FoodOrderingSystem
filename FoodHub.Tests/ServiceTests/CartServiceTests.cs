using AutoMapper;
using FoodHub.DTOs.Cart;
using FoodHub.Models;
using FoodHub.Profiles;
using FoodHub.Repositories;
using FoodHub.Services;
using Moq;

public class CartServiceTests
{
    // ── shared infrastructure ────────────────────────────────────────────────
    private readonly Mock<ICartRepository> _cartRepoMock = new();
    private readonly Mock<IMenuItemRepository> _menuItemRepoMock = new();
    private readonly IMapper _mapper;
    private readonly CartService _sut;

    // A real MenuItem so that nav-prop access (ci.MenuItem.Price) never NPEs
    private readonly MenuItem _menuItem = new()
    {
        Id = 1,
        Name = "Margherita Pizza",
        Price = 10.00m,
        Description = "Classic",
        IsAvailable = true,
        RestaurantId = 1,
        CategoryId = 1
    };

    public CartServiceTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = mapperConfig.CreateMapper();

        _sut = new CartService(_cartRepoMock.Object, _menuItemRepoMock.Object, _mapper);
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Builds a Cart with CartItems whose MenuItem nav prop is fully populated.
    /// Without this, ci.MenuItem.Price throws NullReferenceException inside
    /// CartService.GetCartAsync (line 34) and AddToCartAsync (line 89).
    /// </summary>
    private Cart MakeCartWithItem(string userId, int cartId = 1, int qty = 2)
    {
        var cartItem = new CartItem
        {
            Id = 1,
            CartId = cartId,
            MenuItemId = _menuItem.Id,
            Quantity = qty,
            MenuItem = _menuItem     // ← crucial: nav prop must be set
        };

        return new Cart
        {
            Id = cartId,
            CustomerId = userId,
            CartItems = new List<CartItem> { cartItem }
        };
    }

    // ── GetCartAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetCartAsync_ExistingCart_ReturnsCartDTO()
    {
        const string userId = "user-1";
        var cart = MakeCartWithItem(userId);

        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(userId))
                     .ReturnsAsync(cart);

        var result = await _sut.GetCartAsync(userId);

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(20.00m, result.TotalPrice); // 10.00 * 2
    }

    [Fact]
    public async Task GetCartAsync_NoCart_ReturnsNull()
    {
        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync("missing"))
                     .ReturnsAsync((Cart?)null);

        var result = await _sut.GetCartAsync("missing");

        Assert.Null(result);
    }

    // ── AddToCartAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task AddToCartAsync_NewCart_CreatesCartAndAddsItem()
    {
        const string userId = "user-new";
        var dto = new AddToCartDTO { MenuItemId = _menuItem.Id, Quantity = 1 };

        // Step 1 – no cart exists yet
        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(userId))
                     .ReturnsAsync((Cart?)null);

        // Step 2 – CreateCartAsync returns a cart with Id assigned
        var createdCart = new Cart { Id = 5, CustomerId = userId };
        _cartRepoMock.Setup(r => r.CreateCartAsync(It.IsAny<Cart>()))
                     .ReturnsAsync(createdCart);

        // Step 3 – no existing item
        _cartRepoMock.Setup(r => r.GetCartItemAsync(createdCart.Id, _menuItem.Id))
                     .ReturnsAsync((CartItem?)null);

        _menuItemRepoMock.Setup(r => r.GetByIdAsync(_menuItem.Id))
                         .ReturnsAsync(_menuItem);

        // Step 4 – after adding, GetCartByUserIdAsync returns the cart WITH item
        // (the service calls GetCartAsync internally at the end)
        var cartWithItem = new Cart
        {
            Id = 5,
            CustomerId = userId,
            CartItems = new List<CartItem>
            {
                new CartItem
                {
                    Id = 1, CartId = 5,
                    MenuItemId = _menuItem.Id, Quantity = 1,
                    MenuItem = _menuItem
                }
            }
        };

        // After AddItemAsync is called, switch the mock to return the cart with items
        _cartRepoMock.Setup(r => r.AddItemAsync(It.IsAny<CartItem>()))
                     .Callback(() =>
                     {
                         _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(userId))
                                      .ReturnsAsync(cartWithItem);
                     })
                     .Returns(Task.CompletedTask);

        var result = await _sut.AddToCartAsync(userId, dto);

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(10.00m, result.TotalPrice);
    }

    [Fact]
    public async Task AddToCartAsync_ExistingItem_IncreasesQuantity()
    {
        const string userId = "user-1";
        var dto = new AddToCartDTO { MenuItemId = _menuItem.Id, Quantity = 1 };

        var existingItem = new CartItem
        {
            Id = 1,
            CartId = 1,
            MenuItemId = _menuItem.Id,
            Quantity = 2,
            MenuItem = _menuItem
        };
        var cart = new Cart { Id = 1, CustomerId = userId, CartItems = new List<CartItem> { existingItem } };

        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
        _menuItemRepoMock.Setup(r => r.GetByIdAsync(_menuItem.Id)).ReturnsAsync(_menuItem);
        _cartRepoMock.Setup(r => r.GetCartItemAsync(1, _menuItem.Id)).ReturnsAsync(existingItem);
        _cartRepoMock.Setup(r => r.UpdateCartItemAsync(It.IsAny<CartItem>())).Returns(Task.CompletedTask);

        var result = await _sut.AddToCartAsync(userId, dto);

        Assert.NotNull(result);
        // existingItem.Quantity should now be 3
        Assert.Equal(3, result.Items.First().Quantity);
    }

    [Fact]
    public async Task AddToCartAsync_MenuItemNotFound_ThrowsException()
    {
        const string userId = "user-1";
        var dto = new AddToCartDTO { MenuItemId = 999, Quantity = 1 };
        var cart = new Cart { Id = 1, CustomerId = userId };

        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
        _menuItemRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((MenuItem?)null);

        await Assert.ThrowsAsync<Exception>(() => _sut.AddToCartAsync(userId, dto));
    }

    // ── UpdateQuantityAsync ───────────────────────────────────────────────────

    [Fact]
    public async Task UpdateQuantityAsync_ExistingItem_UpdatesQuantity()
    {
        const string userId = "user-1";

        var cartItem = new CartItem
        {
            Id = 1,
            CartId = 1,
            MenuItemId = _menuItem.Id,
            Quantity = 2,
            MenuItem = _menuItem   // ← nav prop populated; fixes NPE on line 34
        };
        var cart = new Cart
        {
            Id = 1,
            CustomerId = userId,
            CartItems = new List<CartItem> { cartItem }
        };

        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
        _cartRepoMock.Setup(r => r.GetCartItemAsync(1, _menuItem.Id)).ReturnsAsync(cartItem);
        _cartRepoMock.Setup(r => r.UpdateCartItemAsync(It.IsAny<CartItem>()))
                     .Callback<CartItem>(ci => cartItem.Quantity = ci.Quantity)
                     .Returns(Task.CompletedTask);

        var result = await _sut.UpdateQuantityAsync(userId, _menuItem.Id, 5);

        Assert.NotNull(result);
        Assert.Equal(5, result.Items.First().Quantity);
    }

    [Fact]
    public async Task UpdateQuantityAsync_NoCart_ReturnsNull()
    {
        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync("nobody"))
                     .ReturnsAsync((Cart?)null);

        var result = await _sut.UpdateQuantityAsync("nobody", 1, 3);

        Assert.Null(result);
    }

    // ── RemoveItemAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task RemoveItemAsync_ExistingItem_ReturnsTrue()
    {
        const string userId = "user-1";
        var cartItem = new CartItem { Id = 1, CartId = 1, MenuItemId = _menuItem.Id, Quantity = 1, MenuItem = _menuItem };
        var cart = new Cart { Id = 1, CustomerId = userId, CartItems = new List<CartItem> { cartItem } };

        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
        _cartRepoMock.Setup(r => r.GetCartItemAsync(1, _menuItem.Id)).ReturnsAsync(cartItem);
        _cartRepoMock.Setup(r => r.RemoveCartItemAsync(cartItem.Id)).Returns(Task.CompletedTask);

        var result = await _sut.RemoveItemAsync(userId, _menuItem.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveItemAsync_NoCart_ReturnsFalse()
    {
        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync("nobody"))
                     .ReturnsAsync((Cart?)null);

        var result = await _sut.RemoveItemAsync("nobody", 1);

        Assert.False(result);
    }

    // ── ClearCartAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task ClearCartAsync_ExistingCart_ReturnsTrue()
    {
        const string userId = "user-1";
        var cart = new Cart { Id = 1, CustomerId = userId };

        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
        _cartRepoMock.Setup(r => r.ClearCartAsync(1)).Returns(Task.CompletedTask);

        var result = await _sut.ClearCartAsync(userId);

        Assert.True(result);
    }

    [Fact]
    public async Task ClearCartAsync_NoCart_ReturnsFalse()
    {
        _cartRepoMock.Setup(r => r.GetCartByUserIdAsync("nobody"))
                     .ReturnsAsync((Cart?)null);

        var result = await _sut.ClearCartAsync("nobody");

        Assert.False(result);
    }
}