using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace cdi.rejufid.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogosController : ControllerBase
    {
        private readonly ICatalogosService _catalogosService;

        public CatalogosController(ICatalogosService catalogosService)
        {
            _catalogosService = catalogosService;
        }

        [HttpGet("materias")]
        public async Task<ActionResult<IEnumerable<MateriaDTO>>> GetMaterias()
        {
            return Ok(await _catalogosService.GetMaterias());
        }

        [HttpGet("estados")]
        public async Task<ActionResult<IEnumerable<EstadoDTO>>> GetEstados()
        {
            return Ok(await _catalogosService.GetEstados());
        }

        [HttpGet("estatus")]
        public async Task<ActionResult<IEnumerable<EstatusDTO>>> GetEstatus()
        {
            return Ok(await _catalogosService.GetEstatus());
        }

        [HttpGet("organos")]
        public async Task<ActionResult<IEnumerable<OrganoDTO>>> GetOrganos()
        {
            return Ok(await _catalogosService.GetOrganos());
        }

        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetRoles()
        {
            return Ok(await _catalogosService.GetRoles());
        }

        [HttpGet("tipo_asunto")]
        public async Task<ActionResult<IEnumerable<TipoAsuntoDTO>>> GetTiposAsunto()
        {
            return Ok(await _catalogosService.GetTipoAsunto());
        }

        [HttpGet("tipo_organo")]
        public async Task<ActionResult<IEnumerable<TipoOrganoDTO>>> GetTiposOrganos()
        {
            return Ok(await _catalogosService.GetTipoOrgano());
        }

        [HttpGet("tipo_archivo")]
        public async Task<ActionResult<IEnumerable<TipoArchivoDTO>>> GetTiposArchivo()
        {
            return Ok(await _catalogosService.GetTipoArchivo());
        }
    }
}
