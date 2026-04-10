using AppBitacoraVehicular.DTOs.Mantencion;
using AppBitacoraVehicular.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiBitacoraVehicular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class MantencionesController : ControllerBase
    {
        private readonly IMantencionService _mantencionService;

        public MantencionesController(IMantencionService mantencionService)
        {
            _mantencionService = mantencionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _mantencionService.GetAllAsync());
        }

        [HttpGet("proximas")]
        public async Task<IActionResult> GetProximas()
        {
            return Ok(await _mantencionService.GetProximasAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mantencionService.GetByIdAsync(id);
            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMantencionDto request)
        {
            try
            {
                var result = await _mantencionService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.MantencionId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id:int}/cerrar")]
        public async Task<IActionResult> Cerrar(int id, [FromBody] CerrarMantencionDto request)
        {
            try
            {
                var cerrado = await _mantencionService.CerrarAsync(id, request);
                if (!cerrado) return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
