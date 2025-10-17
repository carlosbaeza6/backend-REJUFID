using cdi.rejufid.core.DTOs;
using Microsoft.AspNetCore.Http;
using cdi.rejufid.core.Entities;

namespace cdi.rejufid.core.Interfaces.Repositories
{
    public interface IDocumentoRepository
    {
        Task<string> SavePDFAsync(byte[] contenido, string nombreArchivo);
        Task<int> AddAsync(DocumentoEntity documento);

        Task DeleteByExpedienteAsync(int idExpediente);
        Task<IEnumerable<DocumentoEntity>> GetByExpedienteAsync(int idExpediente);
        Task<DocumentoEntity?> GetByUuidAsync(string uuid);
     


    }

}
