using AutoMapper;
using FoodHub.DTOs.Category;
using FoodHub.DTOs.Restaurant;
using FoodHub.Models;
using FoodHub.Repositories;
using FoodHub.Services;
using NSubstitute;
public class RestaurantServiceTests
{
    private readonly IRestaurantRepository _repo;
    private readonly IMapper _mapper;
    private readonly RestaurantService _sut;

    public RestaurantServiceTests()
    {
        _repo = Substitute.For<IRestaurantRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new RestaurantService(_repo, _mapper);
    }

    [Fact]
    public async Task GetById_Happy_ReturnsRestaurant()
    {
        var r = new Restaurant { Id = 1 };

        _repo.GetByIdAsync(1).Returns(r);
        _mapper.Map<RestaurantDTO>(r).Returns(new RestaurantDTO { Id = 1 });

        var result = await _sut.GetByIdAsync(1);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetById_Sad_ReturnsNull()
    {
        _repo.GetByIdAsync(1).Returns((Restaurant?)null);

        var result = await _sut.GetByIdAsync(1);

        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_Sad_ReturnsFalse()
    {
        _repo.GetByIdAsync(1).Returns((Restaurant?)null);

        var result = await _sut.DeleteAsync(1);

        Assert.False(result);
    }
}