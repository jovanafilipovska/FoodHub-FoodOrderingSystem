namespace FoodHub.DTOs.MenuItem
{
    public class MenuItemDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        public string ImageUrl { get; set; } = "";

        public int RestaurantId { get; set; }

        public int CategoryId { get; set; }
    }
}
