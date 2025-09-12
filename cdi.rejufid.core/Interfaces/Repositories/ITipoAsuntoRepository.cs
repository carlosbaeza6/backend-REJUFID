using cdi.rejufid.core.Entities;

namespace cdi.rejufid.core.Interfaces.Repositories
{
    public interface ITipoAsuntoRepository
    {
        Task<IEnumerable<TipoAsuntoEntity>> GetAllAsync();
    }
}
