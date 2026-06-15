using FoodHub.Controllers;
using FoodHub.DTOs.Restaurant;
using FoodHub.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;

public class RestaurantControllerTests
{
    private readonly IRestaurantService _service;
    private readonly RestaurantController _sut;

    public RestaurantControllerTests()
    {
        _service = Substitute.For<IRestaurantService>();
        _sut = new RestaurantController(_service);
    }

    // ================= GET ALL =================

    [Fact]
    public async Task GetAll_Returns200()
    {
        _service.GetAllAsync().Returns(new List<RestaurantDTO>());

        var result = await _sut.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    // ================= GET BY ID =================

    [Fact]
    public async Task GetById_Happy_Returns200()
    {
        _service.GetByIdAsync(1).Returns(new RestaurantDTO());

        var result = await _sut.GetById(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_Sad_Returns404()
    {
        _service.GetByIdAsync(1).Returns((RestaurantDTO?)null);

        var result = await _sut.GetById(1);

        Assert.IsType<NotFoundResult>(result);
    }

    // ================= CREATE (AUTH) =================

    [Fact]
    public async Task Create_NoUser_Returns401()
    {
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var result = await _sut.Create(new CreateRestaurantDTO());

        Assert.IsType<UnauthorizedResult>(result);
    }

    // ================= UPDATE =================

    [Fact]
    public async Task Update_NotFound_Returns404()
    {
        _service.UpdateAsync(1, Arg.Any<UpdateRestaurantDTO>())
            .Returns((RestaurantDTO?)null);

        var result = await _sut.Update(1, new UpdateRestaurantDTO());

        Assert.IsType<NotFoundResult>(result);
    }

    // ================= DELETE =================

    [Fact]
    public async Task Delete_Happy_Returns204()
    {
        _service.DeleteAsync(1).Returns(true);

        var result = await _sut.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_Sad_Returns404()
    {
        _service.DeleteAsync(1).Returns(false);

        var result = await _sut.Delete(1);

        Assert.IsType<NotFoundResult>(result);
    }
}