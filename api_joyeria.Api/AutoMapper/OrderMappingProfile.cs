using AutoMapper;
using api_joyeria.Domain.Entities;
using api_joyeria.Application.DTOs;

namespace api_joyeria.Api.AutoMapper;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<OrderItem, OrderItemDto>();
        CreateMap<Order, OrderDto>()
                         .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items));
    }
}