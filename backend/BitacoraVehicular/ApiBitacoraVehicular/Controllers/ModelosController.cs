using AppBitacoraVehicular.DTOs.Modelo;
using AppBitacoraVehicular.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiBitacoraVehicular.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ModelosController : ControllerBase
    {
        private readonly IModeloService _modeloService;

        public ModelosController(IModeloService modeloService)
        {
            _modeloService = modeloService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _modeloService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _modeloService.GetByIdAsync(id);
            if (result is null) return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateModeloDto request)
        {
            try
            {
                var result = await _modeloService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.ModeloId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateModeloDto request)
        {
            try
            {
                var actualizado = await _modeloService.UpdateAsync(id, request);
                if (!actualizado) return NotFound();

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _modeloService.DeleteAsync(id);
            if (!eliminado) return NotFound();

            return NoContent();
        }
    }
}
