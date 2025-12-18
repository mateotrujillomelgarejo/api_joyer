using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces;

namespace api_joyeria.Application.Services;

public class OrderService : IOrderService
{
    public Order CreateOrder(Cart cart, GuestCheckoutDto checkoutDetails)
    {
        if (cart == null || cart.ExpirationDate < DateTime.UtcNow)
            throw new Exception("Cart is invalid or expired");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CartId = cart.Id,
            CustomerDetails = new OrderCustomer
            {
                Email = checkoutDetails.Email,
                ShippingAddress = checkoutDetails.ShippingAddress
            },
            Items = cart.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList(),
            Total = cart.Total,
            Status = OrderStatus.Pending
        };

        return order;
    }
}