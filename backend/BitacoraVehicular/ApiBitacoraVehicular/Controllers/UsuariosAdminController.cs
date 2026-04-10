using AppBitacoraVehicular.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiBitacoraVehicular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class UsuariosAdminController : ControllerBase
    {
        private readonly IUsuarioAdminService _usuarioAdminService;

        public UsuariosAdminController(IUsuarioAdminService usuarioAdminService)
        {
            _usuarioAdminService = usuarioAdminService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _usuarioAdminService.GetAllAsync());
        }

        [HttpGet("pendientes")]
        public async Task<IActionResult> GetPendientes()
        {
            return Ok(await _usuarioAdminService.GetPendientesAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _usuarioAdminService.GetByIdAsync(id);
            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpPut("{id:int}/activar")]
        public async Task<IActionResult> Activar(int id)
        {
            try
            {
                var activado = await _usuarioAdminService.ActivarAsync(id);
                if (!activado) return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id:int}/inactivar")]
        public async Task<IActionResult> Inactivar(int id)
        {
            try
            {
                var inactivado = await _usuarioAdminService.InactivarAsync(id);
                if (!inactivado) return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
