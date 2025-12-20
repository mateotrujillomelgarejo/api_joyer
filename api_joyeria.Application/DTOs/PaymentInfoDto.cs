using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_joyeria.Application.DTOs
{
    // api_joyeria.Application.DTOs/Payment/PaymentInfoDto.cs
    public class PaymentInfoDto
    {
        public int PaymentId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime AttemptedAt { get; set; }
        public string? GatewayResponse { get; set; }
    }
}
