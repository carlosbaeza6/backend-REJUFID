using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Entities;

namespace cdi.rejufid.core.Interfaces.Services
{
    public interface IUsuarioRolService
    {
        Task<UsuarioRolDTO?> GetByEmailAsync(string correo);
        Task<IEnumerable<UsuarioRolDTO>> GetAllAsync();
        Task<UsuarioRolDTO?> GetByIdAsync(int id);
        Task<int> CreateAsync(UsuarioRolDTO dto);
        Task<bool> UpdateAsync(int id, UsuarioRolDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
