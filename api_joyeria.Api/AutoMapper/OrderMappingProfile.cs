using AutoMapper;
using api_joyeria.Domain.Entities;
using api_joyeria.Api.Dtos;

namespace api_joyeria.Api.AutoMapper
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(d => d.OrderId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Total, o => o.MapFrom(s => s.TotalAmount.Amount))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ProductId))
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity))
                .ForMember(d => d.UnitPrice, o => o.MapFrom(s => s.UnitPrice.Amount));
        }
    }
}