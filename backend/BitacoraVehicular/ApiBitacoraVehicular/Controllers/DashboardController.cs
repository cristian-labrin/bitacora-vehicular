using AppBitacoraVehicular.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiBitacoraVehicular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen()
        {
            var result = await _dashboardService.GetResumenAsync();
            return Ok(result);
        }

        [HttpGet("viajes-abiertos")]
        public async Task<IActionResult> GetViajesAbiertos()
        {
            return Ok(await _dashboardService.GetViajesAbiertosAsync());
        }

        [HttpGet("usuarios-pendientes")]
        public async Task<IActionResult> GetUsuariosPendientes()
        {
            return Ok(await _dashboardService.GetUsuariosPendientesAsync());
        }

        [HttpGet("vehiculos-proximos-mantencion")]
        public async Task<IActionResult> GetVehiculosProximosMantencion()
        {
            return Ok(await _dashboardService.GetVehiculosProximosMantencionAsync());
        }

        [HttpGet("vehiculos-fuera-servicio")]
        public async Task<IActionResult> GetVehiculosFueraServicio()
        {
            return Ok(await _dashboardService.GetVehiculosFueraServicioAsync());
        }
    }
}
