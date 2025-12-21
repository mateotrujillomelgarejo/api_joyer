namespace api_joyeria.Domain.Entities
{
    // Clase simple que representa la dirección de envío usada por la orden.
    public sealed class ShippingAddress
    {
        public string RecipientName { get; private set; }
        public string Line1 { get; private set; }
        public string Line2 { get; private set; }
        public string City { get; private set; }
        public string PostalCode { get; private set; }
        public string Country { get; private set; }

        public ShippingAddress(string recipientName, string line1, string line2, string city, string postalCode, string country)
        {
            RecipientName = recipientName;
            Line1 = line1;
            Line2 = line2;
            City = city;
            PostalCode = postalCode;
            Country = country;
        }
    }
}