using System.ComponentModel.DataAnnotations;

namespace api_joyeria.Application.DTOs.Payment
{
    // api_joyeria.Application.DTOs/Payment/PaymentRequestDto.cs
    public class PaymentRequestDto
    {
        [Required] public string IdempotencyKey { get; set; } = string.Empty;
        [Required] public string Method { get; set; } = string.Empty; // e.g. "izipay_card"
        public string? ReturnUrl { get; set; } // for redirect flows if needed
    }
}
