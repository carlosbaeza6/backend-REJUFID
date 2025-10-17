using cdi.rejufid.core.DTOs;

namespace cdi.rejufid.core.Interfaces.Services
{
    public interface IFirmaDigitalService
    {
        Task<FirmaRespuestaDTO> FirmarArchivoAsync(byte[] documentoPdf, FirmaSolicitudDTO dto);
    }
}