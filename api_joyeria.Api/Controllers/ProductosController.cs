using api_joyeria.Application.DTOs;
using api_joyeria.Application.Interfaces.Repositories;
using api_joyeria.Application.Interfaces.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace api_joyeria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;
        private readonly IStorageService _storageService;

        public ProductosController(IProductoRepository productoRepository, IMapper mapper, IStorageService storageService)
        {
            _productoRepository = productoRepository;
            _mapper = mapper;
            _storageService = storageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken cancellationToken = default)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize <= 0 ? 50 : (pageSize > 200 ? 200 : pageSize);

            var productos = await _productoRepository.GetPagedAsync(page, pageSize, cancellationToken);
            var dto = _mapper.Map<IEnumerable<ProductoDto>>(productos);
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetById(string id, CancellationToken cancellationToken)
        {
            var p = await _productoRepository.GetByIdAsync(id, cancellationToken);
            if (p == null) return NotFound();
            var dto = _mapper.Map<ProductoDto>(p);
            return Ok(dto);
        }

        [HttpPost("{id}/image")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> UploadImage(string id, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0) return BadRequest("Archivo no proporcionado");
            var producto = await _productoRepository.GetByIdAsync(id, cancellationToken);
            if (producto == null) return NotFound();

            // Subir a Cloudinary y obtener URL
            string imageUrl;
            try
            {
                imageUrl = await _storageService.UploadImageAsync(file, cancellationToken);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }

            // Actualizar en el repositorio
            await _productoRepository.UpdateImageUrlAsync(id, imageUrl, cancellationToken);

            return Ok(new { imageUrl });
        }
    }
}