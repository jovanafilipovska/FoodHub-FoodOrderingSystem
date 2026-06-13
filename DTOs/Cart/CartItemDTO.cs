namespace FoodHub.DTOs.Cart
{
    public class CartItemDTO
    {
        public int Id { get; set; }

        public int MenuItemId { get; set; }

        public string MenuItemName { get; set; } = "";

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public decimal SubTotal { get; set; }
    }
}
