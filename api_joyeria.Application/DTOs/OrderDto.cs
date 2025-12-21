using System.Collections.Generic;

namespace api_joyeria.Api.Dtos
{
    public class OrderDto
    {
        public string OrderId { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
    }

    public class OrderItemDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}