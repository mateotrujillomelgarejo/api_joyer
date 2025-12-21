using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_joyeria.Application.DTOs.Payment
{
    internal class PaymentResponseDto
    {
        public string PaymentId { get; set; }
        public string PaymentUrl { get; set; }
    }
}
