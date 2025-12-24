using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_joyeria.Application.DTOs
{
    using System.Collections.Generic;

    namespace api_joyeria.Api.Dtos
    {
        public class CartDto
        {
            public string Id { get; set; } = default!;
            public List<CartItemDto> Items { get; set; } = new();
            public decimal Subtotal { get; set; }
        }

        public class CartItemDto
        {
            public string ItemId { get; set; } = default!;
            public string ProductId { get; set; } = default!;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public string? ImageUrl { get; set; }
        }

        public class AddCartItemDto
        {
            public string? CartId { get; set; }
            public string ProductId { get; set; }
            public int Quantity { get; set; }
        }

        public class UpdateCartItemDto
        {
            public string CartId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
