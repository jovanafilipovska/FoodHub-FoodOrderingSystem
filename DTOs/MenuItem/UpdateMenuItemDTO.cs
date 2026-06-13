using System.ComponentModel.DataAnnotations;

namespace FoodHub.DTOs.MenuItem
{
    public class UpdateMenuItemDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [StringLength(500)]
        public string Description { get; set; } = "";

        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }

        [Url]
        public string ImageUrl { get; set; } = "";

        [Required]
        public int RestaurantId { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
