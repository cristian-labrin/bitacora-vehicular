using AppBitacoraVehicular.DTOs.CargaCombustible;
using AppBitacoraVehicular.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiBitacoraVehicular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CargasCombustibleController : ControllerBase
    {
        private readonly ICargaCombustibleService _cargaCombustibleService;

        public CargasCombustibleController(ICargaCombustibleService cargaCombustibleService)
        {
            _cargaCombustibleService = cargaCombustibleService;
        }

        [HttpGet("mis-cargas")]
        public async Task<IActionResult> GetMisCargas()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            return Ok(await _cargaCombustibleService.GetMisCargasAsync(usuarioId));
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _cargaCombustibleService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var esAdmin = User.IsInRole("Administrador");

            var result = await _cargaCombustibleService.GetByIdAsync(id, usuarioId, esAdmin);
            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCargaCombustibleDto request)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _cargaCombustibleService.CreateAsync(usuarioId, request);

                return CreatedAtAction(nameof(GetById), new { id = result.CargaCombustibleId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
