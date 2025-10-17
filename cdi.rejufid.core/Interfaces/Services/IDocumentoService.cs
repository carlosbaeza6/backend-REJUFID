using cdi.rejufid.core.DTOs;
using Microsoft.AspNetCore.Http;

namespace cdi.rejufid.core.Interfaces.Services
{
    public interface IDocumentoService
    {
        Task<int> CreateAsync(DocumentoEntity documento);
   
        Task DeleteByExpedienteAsync(int idExpediente);
        Task<IEnumerable<DocumentoRespuestaDTO>> GetByExpedienteAsync(int idExpediente);
        Task<string> SavePDFAsync(byte[] contenido, string nombreArchivo);
        Task<VerificacionDocumentoDTO> VerificarPdfAsync(byte[] pdfBytes);
        Task<List<DocumentoEntity>> PrepareAndSignDocumentsAsync(
            List<(IFormFile Archivo, int IdTipoArchivo)> archivos,
            FirmaSolicitudDTO firmaDto);





    }
}
