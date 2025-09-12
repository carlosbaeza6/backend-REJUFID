using cdi.rejufid.core.Entities;

namespace cdi.rejufid.core.Interfaces.Repositories
{
    public interface ITipoOrganoRepository
    {
        Task<IEnumerable<TipoOrganoEntity>> GetAllAsync();
    }
}
