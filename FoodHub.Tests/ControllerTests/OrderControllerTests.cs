using FoodHub.Controllers;
using FoodHub.DTOs.Order;
using FoodHub.Models;
using FoodHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;

public class OrderControllerTests
{
    private readonly IOrderService _service;
    private readonly OrderController _sut;

    public OrderControllerTests()
    {
        _service = Substitute.For<IOrderService>();
        _sut = new OrderController(_service);
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

    // =========================
    // GET BY ID
    // =========================

    [Fact]
    public async Task GetById_Happy_Returns200()
    {
        _service.GetByIdAsync(1).Returns(new OrderDTO());

        var result = await _sut.GetById(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        _service.GetByIdAsync(1).Returns((OrderDTO?)null);

        var result = await _sut.GetById(1);

        Assert.IsType<NotFoundResult>(result);
    }

    // =========================
    // MY ORDERS
    // =========================

    [Fact]
    public async Task GetMyOrders_Happy_Returns200()
    {
        SetUser("u1");

        _service.GetByUserAsync("u1")
            .Returns(new List<OrderDTO>());

        var result = await _sut.GetMyOrders();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetMyOrders_NoUser_Returns401()
    {
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var result = await _sut.GetMyOrders();

        Assert.IsType<UnauthorizedResult>(result);
    }

    // =========================
    // CHECKOUT
    // =========================

    [Fact]
    public async Task Checkout_Happy_Returns200()
    {
        SetUser("u1");

        _service.CheckoutAsync("u1")
            .Returns(new OrderDTO());

        var result = await _sut.Checkout();

        Assert.IsType<OkObjectResult>(result);
    }

    // =========================
    // UPDATE STATUS
    // =========================

    [Fact]
    public async Task UpdateStatus_InvalidEnum_Returns400()
    {
        var dto = new UpdateOrderStatusDTO
        {
            Status = "INVALID"
        };

        var result = await _sut.UpdateStatus(1, dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_Happy_Returns200()
    {
        _service.UpdateStatusAsync(1, Arg.Any<OrderStatus>())
            .Returns(new OrderDTO());

        var dto = new UpdateOrderStatusDTO
        {
            Status = "Pending"
        };

        var result = await _sut.UpdateStatus(1, dto);

        Assert.IsType<OkObjectResult>(result);
    }

    // =========================
    // CANCEL ORDER
    // =========================

    [Fact]
    public async Task CancelOrder_Happy_Returns200()
    {
        SetUser("u1");

        _service.CancelOrderAsync(1, "u1")
            .Returns(true);

        var result = await _sut.CancelOrder(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task CancelOrder_Failed_Returns404()
    {
        SetUser("u1");

        _service.CancelOrderAsync(1, "u1")
            .Returns(false);

        var result = await _sut.CancelOrder(1);

        Assert.IsType<NotFoundResult>(result);
    }
}