using cdi.rejufid.core.Entities;

namespace cdi.rejufid.core.Interfaces.Repositories
{
    public interface ITipoArchivoRepository
    {
        Task<IEnumerable<TipoArchivoEntity>> GetAllAsync();
    }
}
