using cdi.rejufid.core.Entities;

namespace cdi.rejufid.core.Interfaces.Repositories
{
    public interface IEstatusRepository
    {
        Task<IEnumerable<EstatusEntity>> GetAllAsync();
    }
}
