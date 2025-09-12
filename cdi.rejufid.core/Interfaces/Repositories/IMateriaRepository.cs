using cdi.rejufid.core.Entities;

namespace cdi.rejufid.core.Interfaces.Repositories
{
    public interface IMateriaRepository
    {
        Task<IEnumerable<MateriaEntity>> GetAllAsync();
    }
}
