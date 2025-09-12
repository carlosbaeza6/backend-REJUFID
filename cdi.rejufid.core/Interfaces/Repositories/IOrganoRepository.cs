using cdi.rejufid.core.Entities;

namespace cdi.rejufid.core.Interfaces.Repositories
{
    public interface IOrganoRepository
    {
        Task<IEnumerable<OrganoEntity>> GetAllAsync();
    }
}
