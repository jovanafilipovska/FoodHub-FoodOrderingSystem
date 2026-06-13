using System.ComponentModel.DataAnnotations;

namespace FoodHub.DTOs.Category
{
    public class CreateCategoryDTO
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = "";
    }
}
