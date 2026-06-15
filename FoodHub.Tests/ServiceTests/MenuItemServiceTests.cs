using AutoMapper;
using FoodHub.DTOs.Category;
using FoodHub.DTOs.MenuItem;
using FoodHub.Models;
using FoodHub.Repositories;
using FoodHub.Services;
using NSubstitute;
public class MenuItemServiceTests
{
    private readonly IMenuItemRepository _menu;
    private readonly IRestaurantRepository _rest;
    private readonly ICategoryRepository _cat;
    private readonly IMapper _mapper;
    private readonly MenuItemService _sut;

    public MenuItemServiceTests()
    {
        _menu = Substitute.For<IMenuItemRepository>();
        _rest = Substitute.For<IRestaurantRepository>();
        _cat = Substitute.For<ICategoryRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new MenuItemService(_menu, _rest, _cat, _mapper);
    }

    // GET ALL
    [Fact]
    public async Task GetAll_Happy_ReturnsData()
    {
        _menu.GetAllAsync().Returns(new List<MenuItem>());
        _mapper.Map<IEnumerable<MenuItemDTO>>(Arg.Any<List<MenuItem>>())
            .Returns(new List<MenuItemDTO>());

        var result = await _sut.GetAllAsync();

        Assert.Empty(result);
    }

    // GET BY ID
    [Fact]
    public async Task GetById_Happy_ReturnsItem()
    {
        var item = new MenuItem { Id = 1 };

        _menu.GetByIdAsync(1).Returns(item);
        _mapper.Map<MenuItemDTO>(item).Returns(new MenuItemDTO { Id = 1 });

        var result = await _sut.GetByIdAsync(1);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetById_Sad_ReturnsNull()
    {
        _menu.GetByIdAsync(1).Returns((MenuItem?)null);

        var result = await _sut.GetByIdAsync(1);

        Assert.Null(result);
    }

    // CREATE
    [Fact]
    public async Task Create_Happy_ReturnsItem()
    {
        var dto = new CreateMenuItemDTO
        {
            RestaurantId = 1,
            CategoryId = 1,
            Price = 10
        };

        _rest.GetByIdAsync(1).Returns(new Restaurant());
        _cat.GetByIdAsync(1).Returns(new Category());
        _mapper.Map<MenuItem>(dto).Returns(new MenuItem());

        _mapper.Map<MenuItemDTO>(Arg.Any<MenuItem>())
            .Returns(new MenuItemDTO());

        var result = await _sut.CreateAsync(dto);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Create_Sad_RestaurantMissing_Throws()
    {
        _rest.GetByIdAsync(1).Returns((Restaurant?)null);

        var dto = new CreateMenuItemDTO
        {
            RestaurantId = 1,
            CategoryId = 1,
            Price = 10
        };

        await Assert.ThrowsAsync<Exception>(() => _sut.CreateAsync(dto));
    }

    [Fact]
    public async Task Create_Sad_InvalidPrice_Throws()
    {
        _rest.GetByIdAsync(1).Returns(new Restaurant());
        _cat.GetByIdAsync(1).Returns(new Category());

        var dto = new CreateMenuItemDTO
        {
            RestaurantId = 1,
            CategoryId = 1,
            Price = 0
        };

        await Assert.ThrowsAsync<Exception>(() => _sut.CreateAsync(dto));
    }

    // UPDATE
    [Fact]
    public async Task Update_Sad_ReturnsNull()
    {
        _menu.GetByIdAsync(1).Returns((MenuItem?)null);

        var result = await _sut.UpdateAsync(1, new UpdateMenuItemDTO());

        Assert.Null(result);
    }
}