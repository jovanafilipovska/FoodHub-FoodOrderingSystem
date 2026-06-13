using System.ComponentModel.DataAnnotations;

namespace FoodHub.DTOs.Cart
{
    public class UpdateCartItemDTO
    {
        [Range(1, 100)]
        public int Quantity { get; set; }
    }
}
