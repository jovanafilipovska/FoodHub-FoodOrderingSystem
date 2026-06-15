using FoodHub.Controllers;
using FoodHub.DTOs.Category;
using FoodHub.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

public class CategoryControllerTests
{
    private readonly ICategoryService _service;
    private readonly CategoryController _sut;

    public CategoryControllerTests()
    {
        _service = Substitute.For<ICategoryService>();
        _sut = new CategoryController(_service);
    }

    // ================= GET ALL =================

    [Fact]
    public async Task GetAll_Returns200_WithData()
    {
        _service.GetAllAsync().Returns(new List<CategoryDTO>
        {
            new() { Id = 1, Name = "Pizza" }
        });

        var result = await _sut.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    // ================= GET BY ID =================

    [Fact]
    public async Task GetById_Valid_Returns200()
    {
        _service.GetByIdAsync(1).Returns(new CategoryDTO { Id = 1 });

        var result = await _sut.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        _service.GetByIdAsync(1).Returns((CategoryDTO?)null);

        var result = await _sut.GetById(1);

        Assert.IsType<NotFoundResult>(result);
    }

    // ================= CREATE =================

    [Fact]
    public async Task Create_Returns200()
    {
        var dto = new CreateCategoryDTO { Name = "Pizza" };

        _service.CreateAsync(dto)
            .Returns(new CategoryDTO { Id = 1, Name = "Pizza" });

        var result = await _sut.Create(dto);

        Assert.IsType<OkObjectResult>(result);
    }

    // ================= UPDATE =================

    [Fact]
    public async Task Update_Valid_Returns200()
    {
        _service.UpdateAsync(1, Arg.Any<UpdateCategoryDTO>())
            .Returns(new CategoryDTO { Id = 1 });

        var result = await _sut.Update(1, new UpdateCategoryDTO());

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Update_NotFound_Returns404()
    {
        _service.UpdateAsync(1, Arg.Any<UpdateCategoryDTO>())
            .Returns((CategoryDTO?)null);

        var result = await _sut.Update(1, new UpdateCategoryDTO());

        Assert.IsType<NotFoundResult>(result);
    }

    // ================= DELETE =================

    [Fact]
    public async Task Delete_Valid_Returns204()
    {
        _service.DeleteAsync(1).Returns(true);

        var result = await _sut.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_NotFound_Returns404()
    {
        _service.DeleteAsync(1).Returns(false);

        var result = await _sut.Delete(1);

        Assert.IsType<NotFoundResult>(result);
    }
}