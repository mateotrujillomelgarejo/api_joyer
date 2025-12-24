using api_joyeria.Api.Controllers;
using api_joyeria.Api.Dtos;
using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Domain.Entities;
using api_joyeria.Domain.ValueObjects;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace api_joyeria.Tests.Api
{
    public class ProductosControllerTests
    {
        [Fact]
        public async Task GetAll_ReturnsOkWithMappedDtos()
        {
            var repo = new Mock<IProductoRepository>();
            var mapper = new Mock<IMapper>();

            var productos = new List<Producto> {
                new Producto("p1","N","D", Money.Of(10m,"USD"), 5)
            };

            repo.Setup(r => r.GetAllAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(productos);

            mapper.Setup(m => m.Map<IEnumerable<ProductoDto>>(It.IsAny<IEnumerable<Producto>>()))
                  .Returns(new List<ProductoDto> {
              new ProductoDto { Id = "p1", Nombre = "N", Precio = 10m, Stock = 5 }
                  });

            var controller = new ProductosController(repo.Object, mapper.Object);

            // 🔥 Forma correcta
            var actionResult = await controller.GetAll();
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<ProductoDto>>(okResult.Value);

            Assert.Single(list);
        }


        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            var repo = new Mock<IProductoRepository>();
            var mapper = new Mock<IMapper>();

            repo.Setup(r => r.GetByIdAsync("x", It.IsAny<CancellationToken>())).ReturnsAsync((Producto)null);

            var controller = new ProductosController(repo.Object, mapper.Object);
            var result = await controller.GetById("x", CancellationToken.None);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            var repo = new Mock<IProductoRepository>();
            var mapper = new Mock<IMapper>();

            var producto = new Producto("p1", "N", "D", Money.Of(10m, "USD"), 5);

            repo.Setup(r => r.GetByIdAsync("p1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(producto);

            mapper.Setup(m => m.Map<ProductoDto>(producto))
                  .Returns(new ProductoDto { Id = "p1", Nombre = "N", Precio = 10m, Stock = 5 });

            var controller = new ProductosController(repo.Object, mapper.Object);

            var result = await controller.GetById("p1", CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<ProductoDto>(okResult.Value);

            Assert.Equal("p1", dto.Id);
        }

    }
}