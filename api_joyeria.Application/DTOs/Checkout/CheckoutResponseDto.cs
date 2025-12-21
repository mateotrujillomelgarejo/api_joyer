using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_joyeria.Application.DTOs.Checkout
{
    public class CheckoutResponseDto
    {
        public string OrderId { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
    }

    public class CheckoutResultDto
    {
        public string OrderId { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
    }
}
