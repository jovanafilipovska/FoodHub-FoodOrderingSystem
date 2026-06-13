namespace FoodHub.DTOs.Restaurant
{
    public class RestaurantDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Address { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public string Description { get; set; } = "";

        public string ImageUrl { get; set; } = "";

        public double Rating { get; set; }
        public string OwnerId { get; set; } = "";

        public string OwnerName { get; set; } = "";
    }
}
