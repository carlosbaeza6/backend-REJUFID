using cdi.rejufid.core.Entities;

namespace cdi.rejufid.core.Interfaces.Repositories
{
    public interface IEstadoRepository
    {
        Task<IEnumerable<EstadoEntity>> GetAllAsync();
    }
}

