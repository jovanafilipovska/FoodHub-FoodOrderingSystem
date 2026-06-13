using System.ComponentModel.DataAnnotations;

namespace FoodHub.DTOs.Category
{
    public class UpdateCategoryDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = "";
    }
}
