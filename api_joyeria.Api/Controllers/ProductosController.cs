using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api_joyeria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;

        public ProductosController(IProductoRepository productoRepository, IMapper mapper)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetAll()
        {
            var productos = await _productoRepository.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<ProductoDto>>(productos);
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetById(string id)
        {
            var p = await _productoRepository.GetByIdAsync(id);
            if (p == null) return NotFound();
            var dto = _mapper.Map<ProductoDto>(p);
            return Ok(dto);
        }
    }
}