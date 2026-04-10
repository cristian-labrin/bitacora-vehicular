using AppBitacoraVehicular.DTOs.AsignacionVehiculo;
using AppBitacoraVehicular.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiBitacoraVehicular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class AsignacionesVehiculoController : ControllerBase
    {
        private readonly IAsignacionVehiculoService _asignacionVehiculoService;

        public AsignacionesVehiculoController(IAsignacionVehiculoService asignacionVehiculoService)
        {
            _asignacionVehiculoService = asignacionVehiculoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _asignacionVehiculoService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _asignacionVehiculoService.GetByIdAsync(id);
            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAsignacionVehiculoDto request)
        {
            try
            {
                var result = await _asignacionVehiculoService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.AsignacionVehiculoId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id:int}/finalizar")]
        public async Task<IActionResult> Finalizar(int id, [FromBody] FinalizarAsignacionVehiculoDto request)
        {
            try
            {
                var finalizada = await _asignacionVehiculoService.FinalizarAsync(id, request);
                if (!finalizada) return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
