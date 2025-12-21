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
            public string Id { get; set; }
            public List<CartItemDto> Items { get; set; }
            public decimal Subtotal { get; set; }
        }

        public class CartItemDto
        {
            public string ItemId { get; set; }
            public string ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }

        public class AddCartItemDto
        {
            public string CartId { get; set; }
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
