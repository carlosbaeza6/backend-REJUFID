using System;
using cdi.core;
using cdi.rejufid.core.Interfaces;
using cdi.rejufid.core.Interfaces.Repositories;
using cdi.rejufid.infrastructure.Repositories;

namespace cdi.rejufid.infrastructure
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IDbConnectionFactory context;

        private IExpedienteRepository expedienteRepository;
        private IEstadoRepository estadoRepository;
        private IEstatusRepository estatusRepository;
        private IMateriaRepository materiaRepository;
        private IOrganoRepository organoRepository;
        private IRoleRepository roleRepository;
        private IUsuarioRolRepository usuarioRolRepository;  
        private ITipoAsuntoRepository tipoAsuntoRepository;
        private ITipoOrganoRepository tipoOrganoRepository;
        private ITipoArchivoRepository tipoArchivoRepository;
        private IDocumentoRepository documentoRepository;

        public UnitOfWork(IDbConnectionFactory ctx)
        {
            this.context = ctx;
        }

        public IExpedienteRepository Expedientes
        {
            get
            {
                if (this.expedienteRepository == null)
                    this.expedienteRepository = new ExpedienteRepository(this.context);
                return expedienteRepository;
            }
        }

        public IEstadoRepository Estados
        {
            get
            {
                if (this.estadoRepository == null)
                    this.estadoRepository = new EstadoRepository(this.context);
                return estadoRepository;
            }
        }

        public IEstatusRepository Estatus
        {
            get
            {
                if (this.estatusRepository == null)
                    this.estatusRepository = new EstatusRepository(this.context);
                return estatusRepository;
            }
        }

        public IMateriaRepository Materias
        {
            get
            {
                if (this.materiaRepository == null)
                    this.materiaRepository = new MateriaRepository(this.context);
                return materiaRepository;
            }
        }

        public IOrganoRepository Organos
        {
            get
            {
                if (this.organoRepository == null)
                    this.organoRepository = new OrganoRepository(this.context);
                return organoRepository;
            }
        }

        public IRoleRepository Roles
        {
            get
            {
                if (this.roleRepository == null)
                    this.roleRepository = new RoleRepository(this.context);
                return roleRepository;
            }
        }

        public IUsuarioRolRepository UsuariosRoles
        {
            get
            {
                if (this.usuarioRolRepository == null)
                    this.usuarioRolRepository = new UsuarioRolRepository(this.context);
                return usuarioRolRepository;
            }
        }

        public ITipoAsuntoRepository TipoAsunto
        {
            get
            {
                if (this.tipoAsuntoRepository == null)
                    this.tipoAsuntoRepository = new TipoAsuntoRepository(this.context);
                return tipoAsuntoRepository;
            }
        }

        public ITipoOrganoRepository TipoOrgano
        {
            get
            {
                if (this.tipoOrganoRepository == null)
                    this.tipoOrganoRepository = new TipoOrganoRepository(this.context);
                return tipoOrganoRepository;
            }
        }

        public ITipoArchivoRepository TipoArchivo
        {
            get
            {
                if (this.tipoArchivoRepository == null)
                    this.tipoArchivoRepository = new TipoArchivoRepository(this.context);
                return tipoArchivoRepository;
            }
        }

        public IDocumentoRepository Documentos
        {
            get
            {
                if (this.documentoRepository == null)
                    this.documentoRepository = new DocumentoRepository(this.context);
                return documentoRepository;
            }
        }

        public void Dispose()
        {
            this.context?.Dispose();
        }
    }
}
