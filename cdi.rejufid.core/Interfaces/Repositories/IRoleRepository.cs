using cdi.rejufid.core.Entities;

namespace cdi.rejufid.core.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<RoleEntity>> GetAllAsync();
    }
}
