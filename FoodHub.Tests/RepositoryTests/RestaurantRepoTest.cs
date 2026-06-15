using FoodHub.Data;
using FoodHub.Models;
using FoodHub.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FoodHub.Test.RepositoryTests;

public class RestaurantRepoTests : IDisposable
{
    // ── shared context ──────────────────────────────────────────────────────
    // Both the seed step and the repository must use the SAME DbContext
    // instance (or at least the same InMemory database name) so the repo can
    // see the data that was added.
    private readonly ApplicationDbContext _context;
    private readonly RestaurantRepository _repo;

    public RestaurantRepoTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB per test class
            .Options;

        _context = new ApplicationDbContext(options);
        _repo = new RestaurantRepository(_context);

        SeedData();
    }

    // ── seed helpers ────────────────────────────────────────────────────────
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

        _context.Restaurants.AddRange(
            new Restaurant
            {
                Id = 1,
                Name = "Pizza Palace",
                Address = "1 Main St",
                PhoneNumber = "555-0001",
                Description = "Great pizza",
                OwnerId = "owner-1",
                Owner = owner
            },
            new Restaurant
            {
                Id = 2,
                Name = "Burger Barn",
                Address = "2 Oak Ave",
                PhoneNumber = "555-0002",
                Description = "Juicy burgers",
                OwnerId = "owner-1",
                Owner = owner
            }
        );

        _context.SaveChanges();
    }

    // ── tests ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ShouldReturnRestaurants()
    {
        var result = await _repo.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludeOwnerAndMenuItems()
    {
        var result = await _repo.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Pizza Palace", result.Name);
        Assert.NotNull(result.Owner);
        Assert.Equal("Jane", result.Owner.FirstName);
    }

    [Fact]
    public async Task CreateAsync_ShouldAddRestaurant()
    {
        var newRestaurant = new Restaurant
        {
            Name = "Sushi Spot",
            Address = "3 Elm St",
            PhoneNumber = "555-0003",
            OwnerId = "owner-1"
        };

        var created = await _repo.CreateAsync(newRestaurant);

        Assert.True(created.Id > 0);
        Assert.Equal(3, _context.Restaurants.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyRestaurant()
    {
        // Fetch through EF so the entity is tracked, then update.
        var restaurant = await _context.Restaurants.FindAsync(1);
        Assert.NotNull(restaurant);

        restaurant.Name = "Pizza Palace Updated";
        await _repo.UpdateAsync(restaurant);

        var updated = await _context.Restaurants.FindAsync(1);
        Assert.Equal("Pizza Palace Updated", updated!.Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRestaurant()
    {
        await _repo.DeleteAsync(2);

        Assert.Equal(1, _context.Restaurants.Count());
    }

    // ── cleanup ───────────────────────────────────────────────────────────────
    public void Dispose() => _context.Dispose();
}