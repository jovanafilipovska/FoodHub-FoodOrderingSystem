namespace FoodHub.DTOs.Order
{
    public class OrderItemDTO
    {
        public string MenuItemName { get; set; } = "";

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
    }
}
