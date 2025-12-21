namespace api_joyeria.Domain.ValueObjects
{
    // Reutilizable si necesitas separar Address genérico del ShippingAddress entity.
    public sealed class Address
    {
        public string RecipientName { get; }
        public string Line1 { get; }
        public string Line2 { get; }
        public string City { get; }
        public string PostalCode { get; }
        public string Country { get; }

        public Address(string recipientName, string line1, string line2, string city, string postalCode, string country)
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