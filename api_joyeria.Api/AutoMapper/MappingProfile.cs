using api_joyeria.Api.Dtos; // For CartDto classes
using api_joyeria.Application.Commands.Checkout;
using api_joyeria.Application.Commands.Payments;
using api_joyeria.Application.DTOs;
using api_joyeria.Application.DTOs.api_joyeria.Api.Dtos;
using api_joyeria.Application.DTOs.Checkout;
using api_joyeria.Application.DTOs.Payment;
using api_joyeria.Domain.Entities;
using AutoMapper;

namespace api_joyeria.Api.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // API DTO -> Application Command
            CreateMap<GuestCheckoutDto, CreateGuestOrderCommand>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.ShippingAddress))
                .ForMember(dest => dest.ReturnUrl, opt => opt.MapFrom(src => src.ReturnUrl))
                .ForMember(dest => dest.CancelUrl, opt => opt.MapFrom(src => src.CancelUrl));

            CreateMap<GuestCheckoutItemDto, CreateGuestOrderItemDto>();

            CreateMap<PaymentRequestDto, InitializePaymentCommand>()
                .ForMember(d => d.OrderId, o => o.MapFrom(s => s.OrderId))
                .ForMember(d => d.ReturnUrl, o => o.MapFrom(s => s.ReturnUrl))
                .ForMember(d => d.CancelUrl, o => o.MapFrom(s => s.CancelUrl));

            // Application DTOs -> API DTOs
            CreateMap<CheckoutResponseDto, CheckoutResultDto>()
                .ForMember(d => d.OrderId, o => o.MapFrom(s => s.OrderId))
                .ForMember(d => d.Total, o => o.MapFrom(s => s.Total))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status))
                .ForMember(d => d.Payment, o => o.MapFrom(s => s.Payment));

            CreateMap<PaymentInitResponseDto, PaymentInitResultDto>()
                .ForMember(d => d.PaymentId, o => o.MapFrom(s => s.PaymentId))
                .ForMember(d => d.PaymentUrl, o => o.MapFrom(s => s.PaymentUrl));

            // Domain -> API DTO
            CreateMap<Producto, ProductoDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Nombre, o => o.MapFrom(s => s.Nombre))
                .ForMember(d => d.Descripcion, o => o.MapFrom(s => s.Descripcion))
                .ForMember(d => d.Precio, o => o.MapFrom(s => s.Price.Amount))
                .ForMember(d => d.Stock, o => o.MapFrom(s => s.Stock))
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.ImageUrl));

            // Cart mappings (Domain -> API DTO)
            CreateMap<CartItem, CartItemDto>()
                .ForMember(d => d.ItemId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ProductId))
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity))
                .ForMember(d => d.UnitPrice, o => o.Ignore());

            CreateMap<Cart, CartDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items))
                .ForMember(d => d.Subtotal, o => o.Ignore());
        }
    }
}