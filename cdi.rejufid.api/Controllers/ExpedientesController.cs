using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Interfaces.Services;

namespace cdi.rejufid.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExpedientesController : ControllerBase
    {
        private readonly IExpedienteService _expedienteService;
        private readonly IWebHostEnvironment _env;

        public ExpedientesController(IExpedienteService expedienteService, IWebHostEnvironment env)
        {
            _expedienteService = expedienteService;
            _env = env;
        }

        // GET: /Expedientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpedienteDTO>>> GetAll()
        {
            var lista = await _expedienteService.GetAllAsync();
            return Ok(lista);
        }

        // GET: /Expedientes/Filtros
        [HttpGet("Filtros")]
        public async Task<ActionResult<IEnumerable<ExpedienteDetalleDTO>>> GetFiltered(
            [FromQuery] string? tipoOrgano,
            [FromQuery] string? organo,
            [FromQuery] string? materia,
            [FromQuery] string? palabraClave)
        {
            var hayFiltros =
                !string.IsNullOrWhiteSpace(tipoOrgano) ||
                !string.IsNullOrWhiteSpace(organo) ||
                !string.IsNullOrWhiteSpace(materia) ||
                !string.IsNullOrWhiteSpace(palabraClave);

            var lista = hayFiltros
                ? await _expedienteService.GetFilteredAsync(tipoOrgano, organo, materia, palabraClave)
                : await _expedienteService.GetLast100Async();

            return Ok(lista);
        }

        // GET: /Expedientes/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ExpedienteDTO>> GetById(int id)
        {
            var dto = await _expedienteService.GetByIdAsync(id);
            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        // POST: /Expedientes
        [HttpPost]
        public async Task<ActionResult<ExpedienteDTO>> Create([FromBody] ExpedienteDTO dto)
        {
            var newId = await _expedienteService.CreateAsync(dto);
            dto.Id_expediente = newId;

            return CreatedAtAction(nameof(GetById), new { id = newId }, dto);
        }

        // DELETE: /Expedientes/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var baseUploads = Path.Combine(_env.ContentRootPath, "Archivos");

                var deleted = await _expedienteService.DeleteDeepAsync(id, baseUploads);
                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error al eliminar expediente con id {id}: {ex.Message}");
                return StatusCode(500, "Error interno al eliminar el expediente.");
            }
        }
    }
}
