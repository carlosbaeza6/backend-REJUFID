using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Repositories;
using cdi.rejufid.core.Interfaces.Services;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;

namespace cdi.rejufid.core.Services
{
    public class DocumentoService : IDocumentoService
    {
        private readonly IDocumentoRepository _documentoRepository;
        private readonly IFirmaDigitalService _firmaDigitalService;
        private readonly IMapper _mapper;

        public DocumentoService(
            IDocumentoRepository documentoRepository,
            IFirmaDigitalService firmaDigitalService,
            IMapper mapper)
        {
            _documentoRepository = documentoRepository;
            _firmaDigitalService = firmaDigitalService;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(DocumentoEntity documento)
        {
            return await _documentoRepository.AddAsync(documento);
        }

        public async Task DeleteByExpedienteAsync(int idExpediente)
        {
            var documentos = await _documentoRepository.GetByExpedienteAsync(idExpediente);
            if (documentos == null) return;

            foreach (var doc in documentos)
            {
                var abs = ToAbsolutePath(doc.Ruta_archivo);
                try
                {
                    if (!string.IsNullOrWhiteSpace(abs) && File.Exists(abs))
                        File.Delete(abs);
                }
                catch
                {
                   
                }
            }

            await _documentoRepository.DeleteByExpedienteAsync(idExpediente);
        }

        public async Task<IEnumerable<DocumentoRespuestaDTO>> GetByExpedienteAsync(int idExpediente)
        {
            var documentos = await _documentoRepository.GetByExpedienteAsync(idExpediente);
            return _mapper.Map<IEnumerable<DocumentoRespuestaDTO>>(documentos);
        }

        public async Task<string> SavePDFAsync(byte[] contenido, string nombreArchivo)
        {
            return await _documentoRepository.SavePDFAsync(contenido, nombreArchivo);
        }


        public async Task<List<DocumentoEntity>> PrepareAndSignDocumentsAsync(
        List<(IFormFile Archivo, int IdTipoArchivo)> archivos,
        FirmaSolicitudDTO firmaDto)
        {
            var documentosFinales = new List<DocumentoEntity>();

            foreach (var (archivo, idTipoArchivo) in archivos)
            {
                using var ms = new MemoryStream();
                await archivo.CopyToAsync(ms);
                var contenido = ms.ToArray();

                var doc = new DocumentoEntity
                {
                    Nombre_archivo = archivo.FileName,
                    Id_tipo_archivo = idTipoArchivo,
                    Fecha_carga = DateTime.Now,
                    Esta_firmado = false
                };

                // Firmar si es tipo "Original" (IdTipoArchivo == 1)
                if (idTipoArchivo == 1)
                {
                    var res = await _firmaDigitalService.FirmarArchivoAsync(contenido, firmaDto);

                    contenido = res.DocumentoFirmado;

                    doc.Hash_documento = res.Hash;
                    doc.Uuid_firma = res.Uuid;
                    doc.Usuario_firma = res.UsuarioFirmante;
                    doc.Firma_base64 = res.FirmaBase64;
                    doc.Esta_firmado = true;
                    doc.Fecha_firma = DateTime.Now;
                }

                var nombreArchivoConGuid = $"{Guid.NewGuid()}_{archivo.FileName}";
                doc.Ruta_archivo = await SavePDFAsync(contenido, nombreArchivoConGuid);

                documentosFinales.Add(doc);
            }

            return documentosFinales;
        }



        public async Task<VerificacionDocumentoDTO> VerificarPdfAsync(byte[] pdfBytes)
        {
            var uuid = ExtractUuidFromPdf(pdfBytes);
            if (string.IsNullOrWhiteSpace(uuid))
            {
                return new VerificacionDocumentoDTO
                {
                    Valido = false,
                    Hash = null,
                    Mensaje = "No se encontró un UUID de firma en los metadatos del PDF.",
                    Uuid = null
                };
            }

            var doc = await _documentoRepository.GetByUuidAsync(uuid);
            if (doc is null || !doc.Esta_firmado)
            {
                return new VerificacionDocumentoDTO
                {
                    Valido = false,
                    Hash = null,
                    Mensaje = "UUID no encontrado en BD o el documento no consta como firmado.",
                    Uuid = uuid
                };
            }

            return new VerificacionDocumentoDTO
            {
                Valido = true,
                Hash = doc.Hash_documento,
                IdDocumento = doc.Id_documento,
                IdExpediente = doc.Id_expediente,
                UsuarioFirmante = doc.Usuario_firma,
                FechaFirma = doc.Fecha_firma,
                Mensaje = "Documento válido y firmado.",
                Uuid = uuid
            };
        }

        private static string? ExtractUuidFromPdf(byte[] pdfBytes)
        {
            using var reader = new iTextSharp.text.pdf.PdfReader(pdfBytes);
            var info = reader.Info;
            if (info == null || info.Count == 0) return null;

            if (info.TryGetValue("RejufidUUID", out var uuidMeta) && !string.IsNullOrWhiteSpace(uuidMeta))
            {
                if (Guid.TryParse(uuidMeta.Trim(), out var g))
                    return g.ToString("D");
            }

            if (info.TryGetValue("RejufidUUIDN", out var uuidN) && !string.IsNullOrWhiteSpace(uuidN))
            {
                var n = uuidN.Trim();
                if (Guid.TryParseExact(n, "N", out var gN))
                    return gN.ToString("D");
            }

            return null;
        }


        private static string? ToAbsolutePath(string? rutaPublica)
        {
            if (string.IsNullOrWhiteSpace(rutaPublica)) return null;

            var trimmed = rutaPublica.TrimStart('/', '\\')
                                     .Replace('/', Path.DirectorySeparatorChar)
                                     .Replace('\\', Path.DirectorySeparatorChar);

            if (trimmed.StartsWith("archivos" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                trimmed = trimmed.Substring(("archivos" + Path.DirectorySeparatorChar).Length);

            var baseUploads = Path.Combine(Directory.GetCurrentDirectory(), "archivos");
            return Path.Combine(baseUploads, trimmed);
        }
    }
}
