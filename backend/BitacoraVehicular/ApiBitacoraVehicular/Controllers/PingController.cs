using Microsoft.AspNetCore.Mvc;

namespace ApiBitacoraVehicular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                mensaje = "API Bitácora Vehicular operativa"
            });
        }
    }
}
