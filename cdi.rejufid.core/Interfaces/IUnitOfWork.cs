using cdi.rejufid.core.Interfaces.Repositories;

namespace cdi.rejufid.core.Interfaces
{
    public interface IUnitOfWork: IDisposable
    {
        IExpedienteRepository Expedientes { get; }
        IEstadoRepository Estados { get; }
        IEstatusRepository Estatus { get; }
        IMateriaRepository Materias { get; }
        IOrganoRepository Organos { get; }
        IRoleRepository Roles { get; }
        ITipoAsuntoRepository TipoAsunto { get; }
        ITipoOrganoRepository TipoOrgano { get; }
        ITipoArchivoRepository TipoArchivo { get; }


    }
}
