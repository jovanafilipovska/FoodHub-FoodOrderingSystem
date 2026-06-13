namespace FoodHub.DTOs.Order
{
    public class OrderDTO
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = "";

        public List<OrderItemDTO> Items { get; set; } = new();
    }
}
