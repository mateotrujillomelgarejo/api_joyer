using System;

namespace api_joyeria.Application.Exceptions
{
    public class InsufficientStockException : Exception
    {
        public string ProductId { get; }
        public int Required { get; }
        public int Available { get; }

        public InsufficientStockException(string productId, int required, int available)
            : base($"Insufficient stock for product {productId}. Required: {required}, Available: {available}")
        {
            ProductId = productId;
            Required = required;
            Available = available;
        }
    }
}