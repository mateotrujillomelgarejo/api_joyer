using AutoMapper;
using api_joyeria.Application.DTOs;
using api_joyeria.Domain.Entities;

namespace api_joyeria.Api.AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Producto, ProductoDto>().ReverseMap();
    }
}