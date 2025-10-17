using cdi.rejufid.core.Entities;

namespace cdi.rejufid.core.Interfaces.Repositories
{
    public interface IUsuarioRolRepository
    {
        Task<UsuarioRolEntity?> GetByEmailAsync(string correo);
        Task<IEnumerable<UsuarioRolEntity>> GetAllAsync();
        Task<UsuarioRolEntity?> GetByIdAsync(int id);
        Task<int> CreateAsync(UsuarioRolEntity entity);
        Task<bool> UpdateAsync(UsuarioRolEntity entity);
        Task<bool> DeleteAsync(int id);
    }
}

