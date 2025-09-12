using AutoMapper;
using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Interfaces.Repositories;
using cdi.rejufid.core.Interfaces.Services;

namespace cdi.rejufid.core.Services
{
    public class CatalogosService : ICatalogosService
    {
        private readonly IEstadoRepository estadoRepository;
        private readonly IEstatusRepository estatusRepository;
        private readonly IMateriaRepository materiaRepository;
        private readonly IOrganoRepository organoRepository;
        private readonly IRoleRepository roleRepository;
        private readonly ITipoAsuntoRepository tipoAsuntoRepository;
        private readonly ITipoOrganoRepository tipoOrganoRepository;
        private readonly ITipoArchivoRepository tipoArchivoRepository;
        private readonly IMapper mapper;

        public CatalogosService(
            IEstadoRepository estadoRepository,
            IEstatusRepository estatusRepository,
            IMateriaRepository materiaRepository,
            IOrganoRepository organoRepository,
            IRoleRepository roleRepository,
            ITipoAsuntoRepository tipoAsuntoRepository,
            ITipoOrganoRepository tipoOrganoRepository,
            ITipoArchivoRepository tipoArchivoRepository,
            IMapper mapper)
        {
            this.estadoRepository = estadoRepository;
            this.estatusRepository = estatusRepository;
            this.materiaRepository = materiaRepository;
            this.organoRepository = organoRepository;
            this.roleRepository = roleRepository;
            this.tipoAsuntoRepository = tipoAsuntoRepository;
            this.tipoOrganoRepository = tipoOrganoRepository;
            this.tipoArchivoRepository = tipoArchivoRepository;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<EstadoDTO>> GetEstados()
        {
            var data = await estadoRepository.GetAllAsync();
            return mapper.Map<IEnumerable<EstadoDTO>>(data);
        }

        public async Task<IEnumerable<EstatusDTO>> GetEstatus()
        {
            var data = await estatusRepository.GetAllAsync();
            return mapper.Map<IEnumerable<EstatusDTO>>(data);
        }

        public async Task<IEnumerable<MateriaDTO>> GetMaterias()
        {
            var data = await materiaRepository.GetAllAsync();
            return mapper.Map<IEnumerable<MateriaDTO>>(data);
        }

        public async Task<IEnumerable<OrganoDTO>> GetOrganos()
        {
            var data = await organoRepository.GetAllAsync();
            return mapper.Map<IEnumerable<OrganoDTO>>(data);
        }

        public async Task<IEnumerable<RoleDTO>> GetRoles()
        {
            var data = await roleRepository.GetAllAsync();
            return mapper.Map<IEnumerable<RoleDTO>>(data);
        }

        public async Task<IEnumerable<TipoAsuntoDTO>> GetTipoAsunto()
        {
            var data = await tipoAsuntoRepository.GetAllAsync();
            return mapper.Map<IEnumerable<TipoAsuntoDTO>>(data);
        }

        public async Task<IEnumerable<TipoOrganoDTO>> GetTipoOrgano()
        {
            var data = await tipoOrganoRepository.GetAllAsync();
            return mapper.Map<IEnumerable<TipoOrganoDTO>>(data);
        }

        public async Task<IEnumerable<TipoArchivoDTO>> GetTipoArchivo()
        {
            var data = await tipoArchivoRepository.GetAllAsync();
            return mapper.Map<IEnumerable<TipoArchivoDTO>>(data);
        }
    }
}
