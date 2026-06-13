namespace FoodHub.Models
{
    public class Restaurant
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string ImageUrl { get; set; } = "";

        public double Rating { get; set; }

        public string Address { get; set; } ="";

        public string PhoneNumber { get; set; } ="";

        public string Description { get; set; } = "";
        public string OwnerId { get; set; } = string.Empty;

        public ApplicationUser Owner { get; set; } = null!;

        public ICollection<MenuItem> MenuItems { get; set; }
            = new List<MenuItem>();
    }
}
