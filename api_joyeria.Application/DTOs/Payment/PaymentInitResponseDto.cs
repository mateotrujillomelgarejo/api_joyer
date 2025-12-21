using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_joyeria.Application.DTOs.Payment
{
    public class PaymentInitResponseDto
    {
        public string PaymentId { get; set; }
        public string PaymentUrl { get; set; }
    }

    public class PaymentInitResultDto
    {
        public string PaymentId { get; set; }
        public string PaymentUrl { get; set; }
    }
}
