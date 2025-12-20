using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Application.Services;
using api_joyeria.Domain.Entities;
using api_joyeria.Infrastructure.Repositories;
using AutoMapper;
using Moq;
using System.Threading.Tasks;
using Xunit;

public class ProductoServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_Map_And_Call_Repo()
    {
        var mockRepo = new Mock<IProductoRepository>();
        mockRepo.Setup(r => r.AddAsync(It.IsAny<Producto>(), default)).Returns(Task.CompletedTask);
        mockRepo.Setup(r => r.SaveChangesAsync(default)).ReturnsAsync(1);

        var config = new MapperConfiguration(cfg => cfg.CreateMap<ProductoDto, Producto>().ReverseMap());
        var mapper = config.CreateMapper();

        var service = new ProductoService(mockRepo.Object, mapper);

        var dto = new ProductoDto { Nombre = "Anillo", UnitPrice = 100m, Stock = 5 };
        var result = await service.CreateAsync(dto);

        Assert.Equal(dto.Nombre, result.Nombre);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<Producto>(), default), Times.Once);
        mockRepo.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }
}