namespace api_joyeria.Infrastructure.Payments.Izipay
{
    public class IzipayOptions
    {
        public string BaseUrl { get; set; } = "";
        public string ApiKey { get; set; } = "";
        public string ApiSecret { get; set; } = "";
        public string WebhookSecret { get; set; } = "";
    }
}