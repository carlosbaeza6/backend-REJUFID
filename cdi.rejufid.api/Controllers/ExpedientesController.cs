using Microsoft.AspNetCore.Mvc;
using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Interfaces.Services;

namespace cdi.rejufid.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExpedientesController : ControllerBase
    {
        private readonly IExpedienteService _expedienteService;

        public ExpedientesController(IExpedienteService expedienteService)
        {
            _expedienteService = expedienteService;
        }

        // GET: /Expedientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpedienteDTO>>> GetAll()
        {
            var lista = await _expedienteService.GetAllAsync();
            return Ok(lista);
        }

        // GET: /Expedientes/consulta
        [HttpGet("consulta")]
        public async Task<ActionResult<IEnumerable<ExpedienteDetalleDTO>>> GetFiltered(
            [FromQuery] DateTime? fechaDesde,
            [FromQuery] DateTime? fechaHasta,
            [FromQuery] string? materia,
            [FromQuery] string? estado,
            [FromQuery] string? tipoOrgano,
            [FromQuery] string? palabraClave)
        {
            var hayFiltros = fechaDesde.HasValue || fechaHasta.HasValue ||
                             !string.IsNullOrWhiteSpace(materia) ||
                             !string.IsNullOrWhiteSpace(estado) ||
                             !string.IsNullOrWhiteSpace(tipoOrgano) ||
                             !string.IsNullOrWhiteSpace(palabraClave);

            var lista = hayFiltros
                ? await _expedienteService.GetFilteredAsync(fechaDesde, fechaHasta, materia, estado, tipoOrgano, palabraClave)
                : await _expedienteService.GetUltimos100Async();

            return Ok(lista);
        }

        // GET: /Expedientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExpedienteDTO>> GetById(int id)
        {
            var dto = await _expedienteService.GetByIdAsync(id);
            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        // POST: /Expedientes
        [HttpPost]
        public async Task<ActionResult<ExpedienteDTO>> Create(ExpedienteDTO dto)
        {
            var newId = await _expedienteService.CreateAsync(dto);
            dto.Id_expediente = newId;

            return CreatedAtAction(nameof(GetById), new { id = newId }, dto);
        }

        // PUT: /Expedientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ExpedienteDTO dto)
        {
            if (id != dto.Id_expediente)
                return BadRequest("El ID no coincide");

            var updated = await _expedienteService.UpdateAsync(id, dto);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE: /Expedientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _expedienteService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
