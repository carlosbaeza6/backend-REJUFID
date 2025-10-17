using System.Linq;
using AutoMapper;
using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cdi.rejufid.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentoController : ControllerBase
    {
        private readonly IDocumentoService _documentoService;
        private readonly IMapper _mapper;

        public DocumentoController(IDocumentoService documentoService, IMapper mapper)
        {
            _documentoService = documentoService;
            _mapper = mapper;
        }

        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultiple([FromForm] DocumentoCargaDTO dto)
        {
            try
            {
                if (dto.Archivos == null || dto.Archivos.Count == 0)
                    return BadRequest("No se enviaron archivos.");

                if (dto.TiposArchivo == null || dto.TiposArchivo.Count != dto.Archivos.Count)
                    return BadRequest("Debe indicar el tipo de archivo para cada documento.");

                // Límite de 30 MB por archivo
                for (int i = 0; i < dto.Archivos.Count; i++)
                {
                    var archivo = dto.Archivos[i];
                    if (archivo.Length > 30 * 1024 * 1024)
                        return BadRequest($"El archivo '{archivo.FileName}' excede el límite de 30 MB.");
                }

                // Firma (para documentos tipo Original = 1)
                if (dto.CertificadoCer == null || dto.LlaveKey == null || string.IsNullOrWhiteSpace(dto.PasswordCertificado))
                    return BadRequest("Se requieren certificado, llave y contraseña para firmar documentos tipo 'Original'.");

                // Convertir .cer y .key a byte[]
                using var msCer = new MemoryStream();
                await dto.CertificadoCer.CopyToAsync(msCer);

                using var msKey = new MemoryStream();
                await dto.LlaveKey.CopyToAsync(msKey);

                var firmaDto = new FirmaSolicitudDTO
                {
                    CertificadoCer = msCer.ToArray(),
                    LlaveKey = msKey.ToArray(),
                    PasswordCertificado = dto.PasswordCertificado,
                    RazonFirma = "Firma automática de documentos originales",
                    UbicacionFirma = "REJUFID"
                };

                var archivosConTipo = dto.Archivos
                    .Select((archivo, index) => (archivo, tipo: dto.TiposArchivo[index]))
                    .ToList();

                var documentos = await _documentoService.PrepareAndSignDocumentsAsync(archivosConTipo, firmaDto);

                foreach (var doc in documentos)
                {
                    doc.Id_expediente = dto.Id_expediente;
                    doc.Usuario_carga = dto.Usuario_carga;

                    var id = await _documentoService.CreateAsync(doc);
                    doc.Id_documento = id;
                }

                var respuesta = _mapper.Map<IEnumerable<DocumentoRespuestaDTO>>(documentos);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    mensaje = "Ocurrió un error al guardar los documentos",
                    detalle = ex.Message
                });
            }
        }

        [HttpPost("verificar")]
        public async Task<IActionResult> Verificar(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("No se envió un archivo PDF.");

            try
            {
                using var ms = new MemoryStream();
                await archivo.CopyToAsync(ms);
                var bytes = ms.ToArray();

                var resp = await _documentoService.VerificarPdfAsync(bytes);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al verificar el documento.", detalle = ex.Message });
            }
        }

        [HttpGet("{idExpediente}")]
        public async Task<ActionResult<IEnumerable<DocumentoRespuestaDTO>>> GetByExpediente(int idExpediente)
        {
            var documentos = await _documentoService.GetByExpedienteAsync(idExpediente);
            return Ok(documentos);
        }
    }
}
