using cdi.rejufid.core.DTOs;

namespace cdi.rejufid.core.Interfaces.Services
{
    public interface ICatalogosService
    {
        Task<IEnumerable<EstadoDTO>> GetEstados();
        Task<IEnumerable<EstatusDTO>> GetEstatus();
        Task<IEnumerable<MateriaDTO>> GetMaterias();
        Task<IEnumerable<OrganoDTO>> GetOrganos();
        Task<IEnumerable<RoleDTO>> GetRoles();
        Task<IEnumerable<TipoAsuntoDTO>> GetTipoAsunto();
        Task<IEnumerable<TipoOrganoDTO>> GetTipoOrgano();
        Task<IEnumerable<TipoArchivoDTO>> GetTipoArchivo();
    }
}

