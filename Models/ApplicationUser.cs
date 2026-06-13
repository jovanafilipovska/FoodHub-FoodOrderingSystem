using Microsoft.AspNetCore.Identity;
namespace FoodHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public int LoyaltyPoints { get; set; }

        public ICollection<Order> Orders { get; set; }
        = new List<Order>();

        public Cart? Cart { get; set; }

        public ICollection<Restaurant> OwnedRestaurants { get; set; }
        = new List<Restaurant>();
    }
}
