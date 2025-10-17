using cdi.rejufid.core.Entities;
using cdi.rejufid.core.DTOs;
using AutoMapper;




namespace cdi.rejufid.infrastructure.Mappings
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<ExpedienteEntity, ExpedienteDTO>().ReverseMap();
            CreateMap<EstadoEntity, EstadoDTO>().ReverseMap();
            CreateMap<EstatusEntity, EstatusDTO>().ReverseMap();
            CreateMap<MateriaEntity, MateriaDTO>().ReverseMap();
            CreateMap<OrganoEntity, OrganoDTO>().ReverseMap();
            CreateMap<RoleEntity, RoleDTO>().ReverseMap();
            CreateMap<TipoAsuntoEntity, TipoAsuntoDTO>().ReverseMap();
            CreateMap<TipoOrganoEntity, TipoOrganoDTO>().ReverseMap();
            CreateMap<TipoArchivoEntity, TipoArchivoDTO>().ReverseMap();
            CreateMap<DocumentoEntity, DocumentoRespuestaDTO>();
            CreateMap<UsuarioRolEntity, UsuarioRolDTO>().ReverseMap();


        }
    }
}
