namespace FoodHub.DTOs.Cart
{
    public class CartDTO
    {
        public int Id { get; set; }

        public List<CartItemDTO> Items { get; set; } = new();

        public decimal TotalPrice { get; set; }
    }
}
