using AutoMapper;
using FoodHub.DTOs.Category;
using FoodHub.Models;
using FoodHub.Repositories;
using FoodHub.Services;
using NSubstitute;

namespace FoodHub.Tests.ServiceTests;

public class CategoryServiceTests
{
    private readonly ICategoryRepository _repo;
    private readonly IMapper _mapper;
    private readonly CategoryService _sut;

    public CategoryServiceTests()
    {
        _repo = Substitute.For<ICategoryRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new CategoryService(_repo, _mapper);
    }

    // ================= GET ALL =================

    [Fact]
    public async Task GetAllAsync_HappyPath_ReturnsMappedData()
    {
        var categories = new List<Category> { new() { Id = 1, Name = "Pizza" } };
        var dtos = new List<CategoryDTO> { new() { Id = 1, Name = "Pizza" } };

        _repo.GetAllAsync().Returns(categories);
        _mapper.Map<IEnumerable<CategoryDTO>>(categories).Returns(dtos);

        var result = await _sut.GetAllAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetAllAsync_SadPath_EmptyRepo_ReturnsEmpty()
    {
        _repo.GetAllAsync().Returns(new List<Category>());
        _mapper.Map<IEnumerable<CategoryDTO>>(Arg.Any<List<Category>>())
            .Returns(new List<CategoryDTO>());

        var result = await _sut.GetAllAsync();

        Assert.Empty(result);
    }

    // ================= GET BY ID =================

    [Fact]
    public async Task GetByIdAsync_HappyPath_ReturnsCategory()
    {
        var cat = new Category { Id = 1 };
        var dto = new CategoryDTO { Id = 1 };

        _repo.GetByIdAsync(1).Returns(cat);
        _mapper.Map<CategoryDTO>(cat).Returns(dto);

        var result = await _sut.GetByIdAsync(1);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByIdAsync_SadPath_ReturnsNull()
    {
        _repo.GetByIdAsync(1).Returns((Category?)null);

        var result = await _sut.GetByIdAsync(1);

        Assert.Null(result);
    }

    // ================= CREATE =================

    [Fact]
    public async Task CreateAsync_HappyPath_CreatesCategory()
    {
        var dto = new CreateCategoryDTO { Name = "Pizza" };

        _repo.GetByNameAsync("Pizza").Returns((Category?)null);
        _mapper.Map<Category>(dto).Returns(new Category { Name = "Pizza" });
        _mapper.Map<CategoryDTO>(Arg.Any<Category>())
            .Returns(new CategoryDTO { Name = "Pizza" });

        var result = await _sut.CreateAsync(dto);

        Assert.Equal("Pizza", result.Name);
    }

    [Fact]
    public async Task CreateAsync_SadPath_Duplicate_ThrowsException()
    {
        _repo.GetByNameAsync("Pizza").Returns(new Category());

        var dto = new CreateCategoryDTO { Name = "Pizza" };

        await Assert.ThrowsAsync<Exception>(() => _sut.CreateAsync(dto));
    }

    // ================= UPDATE =================

    [Fact]
    public async Task UpdateAsync_HappyPath_UpdatesCategory()
    {
        var cat = new Category { Id = 1, Name = "Old" };
        var dto = new UpdateCategoryDTO { Name = "New" };

        _repo.GetByIdAsync(1).Returns(cat);
        _mapper.Map<CategoryDTO>(cat).Returns(new CategoryDTO { Name = "New" });

        var result = await _sut.UpdateAsync(1, dto);

        Assert.Equal("New", result!.Name);
    }

    [Fact]
    public async Task UpdateAsync_SadPath_ReturnsNull()
    {
        _repo.GetByIdAsync(1).Returns((Category?)null);

        var result = await _sut.UpdateAsync(1, new UpdateCategoryDTO { Name = "X" });

        Assert.Null(result);
    }

    // ================= DELETE =================

    [Fact]
    public async Task DeleteAsync_HappyPath_ReturnsTrue()
    {
        _repo.GetByIdAsync(1).Returns(new Category { Id = 1 });

        var result = await _sut.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_SadPath_ReturnsFalse()
    {
        _repo.GetByIdAsync(1).Returns((Category?)null);

        var result = await _sut.DeleteAsync(1);

        Assert.False(result);
    }
}