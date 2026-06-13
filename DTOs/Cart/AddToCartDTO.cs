using System.ComponentModel.DataAnnotations;

namespace FoodHub.DTOs.Cart
{
    public class AddToCartDTO
    {
        [Required]
        public int MenuItemId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
