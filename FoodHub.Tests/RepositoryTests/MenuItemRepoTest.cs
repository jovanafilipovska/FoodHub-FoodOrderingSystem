using FoodHub.Data;
using FoodHub.Models;
using FoodHub.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FoodHub.Test.RepositoryTests;

public class MenuItemRepoTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly MenuItemRepository _repo;

    public MenuItemRepoTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repo = new MenuItemRepository(_context);

        SeedData();
    }

    private void SeedData()
    {
        var owner = new ApplicationUser
        {
            Id = "owner-1",
            UserName = "owner@test.com",
            Email = "owner@test.com",
            FirstName = "Jane",
            LastName = "Doe"
        };
        _context.Users.Add(owner);

        var restaurant = new Restaurant
        {
            Id = 10,
            Name = "Test Restaurant",
            Address = "1 Test St",
            PhoneNumber = "555-9999",
            OwnerId = "owner-1",
            Owner = owner
        };
        _context.Restaurants.Add(restaurant);

        var category = new Category
        {
            Id = 5,
            Name = "Main Course"
        };
        _context.Categories.Add(category);

        _context.MenuItems.AddRange(
            new MenuItem
            {
                Id = 1,
                Name = "Margherita Pizza",
                Price = 9.99m,
                Description = "Classic margherita",
                IsAvailable = true,
                RestaurantId = 10,
                Restaurant = restaurant,
                CategoryId = 5,
                Category = category
            },
            new MenuItem
            {
                Id = 2,
                Name = "Cheeseburger",
                Price = 7.49m,
                Description = "Double cheeseburger",
                IsAvailable = true,
                RestaurantId = 10,
                Restaurant = restaurant,
                CategoryId = 5,
                Category = category
            },
            new MenuItem
            {
                Id = 3,
                Name = "Caesar Salad",
                Price = 5.99m,
                Description = "Fresh caesar",
                IsAvailable = false,
                RestaurantId = 10,
                Restaurant = restaurant,
                CategoryId = 5,
                Category = category
            }
        );

        _context.SaveChanges();
    }

    // ── tests ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllItems()
    {
        var result = await _repo.GetAllAsync();

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnItemWithIncludes()
    {
        var result = await _repo.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Margherita Pizza", result.Name);
        Assert.NotNull(result.Restaurant);
        Assert.NotNull(result.Category);
        Assert.Equal("Test Restaurant", result.Restaurant.Name);
    }

    [Fact]
    public async Task GetByRestaurantAsync_ShouldReturnItemsForRestaurant()
    {
        var result = await _repo.GetByRestaurantAsync(10);

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAvailableItemsAsync_ShouldReturnOnlyAvailable()
    {
        var result = await _repo.GetAvailableItemsAsync();

        Assert.Equal(2, result.Count());
        Assert.All(result, item => Assert.True(item.IsAvailable));
    }

    [Fact]
    public async Task CreateAsync_ShouldAddItem()
    {
        var newItem = new MenuItem
        {
            Name = "Tiramisu",
            Price = 4.99m,
            Description = "Italian dessert",
            IsAvailable = true,
            RestaurantId = 10,
            CategoryId = 5
        };

        var created = await _repo.CreateAsync(newItem);

        Assert.True(created.Id > 0);
        Assert.Equal(4, _context.MenuItems.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyItem()
    {
        // Must fetch from the tracked context first, not through a separate repo call
        var item = await _context.MenuItems.FindAsync(1);
        Assert.NotNull(item);

        item.Price = 12.99m;
        await _repo.UpdateAsync(item);

        var updated = await _context.MenuItems.FindAsync(1);
        Assert.Equal(12.99m, updated!.Price);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveItem()
    {
        await _repo.DeleteAsync(3);

        Assert.Equal(2, _context.MenuItems.Count());
    }

    public void Dispose() => _context.Dispose();
}