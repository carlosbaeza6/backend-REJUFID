using Microsoft.AspNetCore.Http;

namespace cdi.rejufid.core.DTOs
{
    public class DocumentoCargaDTO
    {
        public int Id_expediente { get; set; }
        public string Usuario_carga { get; set; }
        public List<IFormFile> Archivos { get; set; }
        public List<int> TiposArchivo { get; set; }
        public IFormFile CertificadoCer { get; set; }
        public IFormFile LlaveKey { get; set; }
        public string PasswordCertificado { get; set; }
    }
}
