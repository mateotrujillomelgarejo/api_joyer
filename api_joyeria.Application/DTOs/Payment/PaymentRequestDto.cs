using System.ComponentModel.DataAnnotations;

namespace api_joyeria.Application.DTOs.Payment
{
    public class PaymentRequestDto
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
