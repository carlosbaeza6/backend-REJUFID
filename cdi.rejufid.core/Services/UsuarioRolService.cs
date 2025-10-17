using AutoMapper;
using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Repositories;
using cdi.rejufid.core.Interfaces.Services;
using BCrypt.Net;

namespace cdi.rejufid.core.Services
{
    public class UsuarioRolService : IUsuarioRolService
    {
        private readonly IUsuarioRolRepository _usuarioRolRepository;
        private readonly IMapper _mapper;

        public UsuarioRolService(IUsuarioRolRepository usuarioRolRepository, IMapper mapper)
        {
            _usuarioRolRepository = usuarioRolRepository;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(UsuarioRolDTO dto)
        {
            var entity = _mapper.Map<UsuarioRolEntity>(dto);

            if (!string.IsNullOrEmpty(dto.Contrasena))
            {
                entity.Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
            }

            return await _usuarioRolRepository.CreateAsync(entity);
        }

        public async Task<bool> UpdateAsync(int id, UsuarioRolDTO dto)
        {
            var entity = _mapper.Map<UsuarioRolEntity>(dto);
            entity.Id_usuario_roles = id;

            if (!string.IsNullOrEmpty(dto.Contrasena))
            {
                entity.Contrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);
            }

            return await _usuarioRolRepository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _usuarioRolRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<UsuarioRolDTO>> GetAllAsync()
        {
            var entities = await _usuarioRolRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UsuarioRolDTO>>(entities);
        }

        public async Task<UsuarioRolDTO?> GetByIdAsync(int id)
        {
            var entity = await _usuarioRolRepository.GetByIdAsync(id);
            return _mapper.Map<UsuarioRolDTO>(entity);
        }

        public async Task<UsuarioRolDTO?> GetByEmailAsync(string correo)
        {
            var entity = await _usuarioRolRepository.GetByEmailAsync(correo);
            return _mapper.Map<UsuarioRolDTO>(entity);
        }
    }
}