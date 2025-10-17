using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Interfaces.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace cdi.rejufid.core.Services
{
    public class FirmaDigitalService : IFirmaDigitalService
    {
        public async Task<FirmaRespuestaDTO> FirmarArchivoAsync(byte[] documentoPdf, FirmaSolicitudDTO dto)
        {
            return await Task.Run(() =>
            {
                using var reader = new PdfReader(documentoPdf);
                using var outputStream = new MemoryStream();
                using var stamper = PdfStamper.CreateSignature(reader, outputStream, '\0', null, true);

                var certificadoX509 = new X509Certificate2(dto.CertificadoCer);
                var llavePrivada = PrivateKeyFactory.DecryptKey(dto.PasswordCertificado.ToCharArray(), dto.LlaveKey);
                var certificadoBC = DotNetUtilities.FromX509Certificate(certificadoX509);

                var hashBase64 = Convert.ToBase64String(SHA256.HashData(dto.CertificadoCer))
                                    .Replace("+", "").Replace("/", "").Replace("=", "");
                var uuidD = Guid.NewGuid().ToString("D"); 
                var uuidN = uuidD.Replace("-", "");       
                var firmante = ExtraerNombre(certificadoX509.Subject);
                var fecha = DateTime.Now;

                var appearance = stamper.SignatureAppearance;
                appearance.SetVisibleSignature(new Rectangle(40f, 18f, 560f, 60f), reader.NumberOfPages, "firma_visible");
                appearance.Reason = dto.RazonFirma ?? "Firma digital REJUFID";
                appearance.Location = dto.UbicacionFirma ?? "REJUFID";
                appearance.SignDate = fecha;

                var bf = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                appearance.Layer2Font = new iTextSharp.text.Font(bf, 8f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
                appearance.Layer2Text =
                    $"FIRMA: {hashBase64}_{uuidD}/{firmante}/{fecha:dd/MM/yyyy HH:mm}"; 

                var info = new Dictionary<string, string>(reader.Info ?? new Dictionary<string, string>())
                {
                    ["RejufidUUID"] = uuidD,                         
                    ["RejufidUUIDN"] = uuidN,                         
                    ["RejufidHash"] = hashBase64,
                    ["RejufidSigner"] = firmante,
                    ["RejufidSigned"] = fecha.ToString("O")            
                };
                stamper.MoreInfo = info;

                var signature = new PrivateKeySignature(llavePrivada, DigestAlgorithms.SHA256);
                var chain = new List<Org.BouncyCastle.X509.X509Certificate> { certificadoBC };
                MakeSignature.SignDetached(appearance, signature, chain, null, null, null, 0, CryptoStandard.CMS);

                var bin = outputStream.ToArray();

                return new FirmaRespuestaDTO
                {
                    DocumentoFirmado = bin,
                    Hash = hashBase64,
                    Uuid = uuidD,           
                    UsuarioFirmante = firmante,
                    FirmaBase64 = Convert.ToBase64String(bin)
                };
            });
        }


        private string ExtraerNombre(string subject)
        {
            var cn = subject.Split(',').FirstOrDefault(p => p.Trim().StartsWith("CN="));
            return cn?.Substring(3).Trim() ?? "FIRMANTE_DESCONOCIDO";
        }
    }
}