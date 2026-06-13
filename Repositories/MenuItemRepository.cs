using FoodHub.Data;
using FoodHub.Models;
using Microsoft.EntityFrameworkCore;
namespace FoodHub.Repositories
{
    public class MenuItemRepository:IMenuItemRepository
    {
        private readonly ApplicationDbContext _context;

        public MenuItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            return await _context.MenuItems
                .Include(m => m.Restaurant)
                .Include(m => m.Category)
                .ToListAsync();
        }

        public async Task<MenuItem?> GetByIdAsync(int id)
        {
            return await _context.MenuItems
                .Include(m => m.Restaurant)
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<MenuItem>> GetByRestaurantAsync(int restaurantId)
        {
            return await _context.MenuItems
                .Where(m => m.RestaurantId == restaurantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetAvailableItemsAsync()
        {
            return await _context.MenuItems
                .Where(m => m.IsAvailable)
                .ToListAsync();
        }

        public async Task<MenuItem> CreateAsync(MenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);

            await _context.SaveChangesAsync();

            return menuItem;
        }

        public async Task UpdateAsync(MenuItem menuItem)
        {
            _context.MenuItems.Update(menuItem);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);

            if (menuItem == null)
                return;

            _context.MenuItems.Remove(menuItem);

            await _context.SaveChangesAsync();
        }
    }
}
