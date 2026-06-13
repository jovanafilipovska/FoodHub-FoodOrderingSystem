using System.ComponentModel.DataAnnotations;

namespace FoodHub.DTOs.Order
{
    public class UpdateOrderStatusDTO
    {
        [Required]
        public string Status { get; set; } = "";
    }
}
