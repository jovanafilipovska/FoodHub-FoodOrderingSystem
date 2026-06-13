using System.ComponentModel.DataAnnotations;

namespace FoodHub.DTOs.Restaurant
{
    public class UpdateRestaurantDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = "";

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = "";

        [StringLength(500)]
        public string Description { get; set; } = "";

        [Url]
        public string ImageUrl { get; set; } = "";
    }
}
