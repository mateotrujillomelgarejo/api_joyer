using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_joyeria.Application.DTOs.Checkout
{
    public class CreateGuestOrderItemDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class GuestCheckoutItemDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
