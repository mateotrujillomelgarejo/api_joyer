using api_joyeria.Domain.ValueObjects;

namespace api_joyeria.Domain.Entities
{
    public sealed class OrderCustomer
    {
        public Email Email { get; private set; }
        public bool IsGuest { get; private set; }

        private OrderCustomer() { }

        public static OrderCustomer CreateGuest(string email)
        {
            return new OrderCustomer
            {
                Email = Email.Of(email),
                IsGuest = true
            };
        }
    }
}