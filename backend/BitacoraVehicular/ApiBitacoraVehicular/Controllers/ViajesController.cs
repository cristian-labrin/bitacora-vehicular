using AppBitacoraVehicular.DTOs.Viaje;
using AppBitacoraVehicular.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiBitacoraVehicular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ViajesController : ControllerBase
    {
        private readonly IViajeService _viajeService;

        public ViajesController(IViajeService viajeService)
        {
            _viajeService = viajeService;
        }

        [HttpGet("mis-viajes")]
        public async Task<IActionResult> GetMisViajes()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Ok(await _viajeService.GetMisViajesAsync(usuarioId));
        }

        [HttpGet("abiertos")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetAbiertos()
        {
            return Ok(await _viajeService.GetViajesAbiertosAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var esAdmin = User.IsInRole("Administrador");

            var result = await _viajeService.GetByIdAsync(id, usuarioId, esAdmin);
            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpPost("iniciar")]
        public async Task<IActionResult> Iniciar([FromBody] CreateViajeDto request)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _viajeService.IniciarViajeAsync(usuarioId, request);

                return CreatedAtAction(nameof(GetById), new { id = result.ViajeId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id:int}/cerrar")]
        public async Task<IActionResult> Cerrar(int id, [FromBody] CerrarViajeDto request)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var esAdmin = User.IsInRole("Administrador");

                var cerrado = await _viajeService.CerrarViajeAsync(id, usuarioId, esAdmin, request);

                if (!cerrado) return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id:int}/cerrar-completo")]
        public async Task<IActionResult> CerrarCompleto(int id, [FromBody] CerrarViajeCompletoDto request)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var esAdmin = User.IsInRole("Administrador");

                var cerrado = await _viajeService.CerrarViajeCompletoAsync(id, usuarioId, esAdmin, request);

                if (!cerrado) return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id:int}/forzar-cierre")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ForzarCierre(int id, [FromBody] ForzarCierreViajeDto request)
        {
            try
            {
                var cerrado = await _viajeService.ForzarCierreAsync(id, request);

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