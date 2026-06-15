using FoodHub.Controllers;
using FoodHub.DTOs.Cart;
using FoodHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;

public class CartControllerTests
{
    private readonly ICartService _service;
    private readonly CartController _sut;

    public CartControllerTests()
    {
        _service = Substitute.For<ICartService>();
        _sut = new CartController(_service);
    }

    private void SetUser(string userId)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }, "mock"));

        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };
    }

    // ================= GET CART =================

    [Fact]
    public async Task GetCart_Happy_Returns200()
    {
        SetUser("u1");

        _service.GetCartAsync("u1").Returns(new CartDTO());

        var result = await _sut.GetCart();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetCart_NoCart_Returns404()
    {
        SetUser("u1");

        _service.GetCartAsync("u1").Returns((CartDTO?)null);

        var result = await _sut.GetCart();

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetCart_NoUser_Returns401()
    {
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var result = await _sut.GetCart();

        Assert.IsType<UnauthorizedResult>(result);
    }

    // ================= ADD =================

    [Fact]
    public async Task AddToCart_Happy_Returns200()
    {
        SetUser("u1");

        _service.AddToCartAsync("u1", Arg.Any<AddToCartDTO>())
            .Returns(new CartDTO());

        var result = await _sut.AddToCart(new AddToCartDTO());

        Assert.IsType<OkObjectResult>(result);
    }

    // ================= UPDATE =================

    [Fact]
    public async Task UpdateQuantity_Happy_Returns200()
    {
        SetUser("u1");

        _service.UpdateQuantityAsync("u1", 1, 2)
            .Returns(new CartDTO());

        var result = await _sut.UpdateQuantity(1, new UpdateCartItemDTO { Quantity = 2 });

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateQuantity_NotFound_Returns404()
    {
        SetUser("u1");

        _service.UpdateQuantityAsync("u1", 1, 2)
            .Returns((CartDTO?)null);

        var result = await _sut.UpdateQuantity(1, new UpdateCartItemDTO { Quantity = 2 });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateQuantity_NegativeQuantity_Returns404()
    {
        // Arrange
        SetUser("u1");

        _service
            .UpdateQuantityAsync("u1", 1, -5)
            .Returns((CartDTO?)null);

        // Act
        var result = await _sut.UpdateQuantity(
            1,
            new UpdateCartItemDTO { Quantity = -5 });

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    // ================= REMOVE =================

    [Fact]
    public async Task Remove_Happy_Returns204()
    {
        SetUser("u1");

        _service.RemoveItemAsync("u1", 1).Returns(true);

        var result = await _sut.RemoveItem(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Remove_Sad_Returns404()
    {
        SetUser("u1");

        _service.RemoveItemAsync("u1", 1).Returns(false);

        var result = await _sut.RemoveItem(1);

        Assert.IsType<NotFoundResult>(result);
    }

    // ================= CLEAR =================

    [Fact]
    public async Task Clear_Happy_Returns204()
    {
        SetUser("u1");

        _service.ClearCartAsync("u1").Returns(true);

        var result = await _sut.ClearCart();

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Clear_Sad_Returns404()
    {
        SetUser("u1");

        _service.ClearCartAsync("u1").Returns(false);

        var result = await _sut.ClearCart();

        Assert.IsType<NotFoundResult>(result);
    }
}