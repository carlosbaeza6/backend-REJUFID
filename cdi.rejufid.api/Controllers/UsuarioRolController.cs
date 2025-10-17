using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Interfaces.Services;

namespace cdi.rejufid.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioRolController : ControllerBase
    {
        private readonly IUsuarioRolService _usuarioRolService;

        public UsuarioRolController(IUsuarioRolService usuarioRolService)
        {
            _usuarioRolService = usuarioRolService;
        }

        // GET: /UsuarioRol
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioRolDTO>>> GetAll()
        {
            var lista = await _usuarioRolService.GetAllAsync();
            return Ok(lista);
        }

        // GET: /UsuarioRol/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioRolDTO>> GetById(int id)
        {
            var dto = await _usuarioRolService.GetByIdAsync(id);
            if (dto == null)
                return NotFound();
            return Ok(dto);
        }

        // GET: /UsuarioRol/correo?email=alguien@ejemplo.com
        [HttpGet("correo")]
        public async Task<ActionResult<UsuarioRolDTO>> GetByEmail([FromQuery] string email)
        {
            var dto = await _usuarioRolService.GetByEmailAsync(email);
            if (dto == null)
                return NotFound();
            return Ok(dto);
        }

        // POST: /UsuarioRol
        [HttpPost]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<UsuarioRolDTO>> Create(UsuarioRolDTO dto)
        {
            var newId = await _usuarioRolService.CreateAsync(dto);
            dto.Id_usuario_roles = newId;
            return CreatedAtAction(nameof(GetById), new { id = newId }, dto);
        }

        // PUT: /UsuarioRol/5
        [HttpPut("{id}")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(int id, UsuarioRolDTO dto)
        {
            if (id != dto.Id_usuario_roles)
                return BadRequest("El ID no coincide");

            var updated = await _usuarioRolService.UpdateAsync(id, dto);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        // DELETE: /UsuarioRol/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _usuarioRolService.DeleteAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
