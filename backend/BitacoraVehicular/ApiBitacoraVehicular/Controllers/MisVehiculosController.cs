using AppBitacoraVehicular.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApiBitacoraVehicular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MisVehiculosController : ControllerBase
    {
        private readonly IMisVehiculosService _misVehiculosService;

        public MisVehiculosController(IMisVehiculosService misVehiculosService)
        {
            _misVehiculosService = misVehiculosService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _misVehiculosService.GetMisVehiculosAsync(usuarioId);
            return Ok(result);
        }
    }
}
