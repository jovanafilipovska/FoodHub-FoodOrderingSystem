using System.ComponentModel.DataAnnotations;

namespace FoodHub.DTOs.Auth
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string FirstName { get; set; } = "";

        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string LastName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(25, MinimumLength = 8)]
        public string Password { get; set; } = "";
        public string Role { get; set; }
    }
}
