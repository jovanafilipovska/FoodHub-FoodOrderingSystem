using FoodHub.Controllers;
using FoodHub.DTOs.Category;
using FoodHub.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

public class MenuItemControllerTests
{
    private readonly IMenuItemService _service;
    private readonly MenuItemController _sut;

    public MenuItemControllerTests()
    {
        _service = Substitute.For<IMenuItemService>();
        _sut = new MenuItemController(_service);
    }

    [Fact]
    public async Task GetAll_Returns200()
    {
        _service.GetAllAsync().Returns(new List<MenuItemDTO>());

        var result = await _sut.GetAll();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        _service.GetByIdAsync(1).Returns((MenuItemDTO?)null);

        var result = await _sut.GetById(1);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_Returns200()
    {
        _service.CreateAsync(Arg.Any<CreateMenuItemDTO>())
            .Returns(new MenuItemDTO());

        var result = await _sut.Create(new CreateMenuItemDTO());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Delete_NotFound_Returns404()
    {
        _service.DeleteAsync(1).Returns(false);

        var result = await _sut.Delete(1);

        Assert.IsType<NotFoundResult>(result);
    }
}