namespace cdi.rejufid.core.DTOs
{
    public class FirmaSolicitudDTO
    {
        public byte[] CertificadoCer { get; set; } = default!;
        public byte[] LlaveKey { get; set; } = default!;
        public string PasswordCertificado { get; set; } = default!;
        public string? RazonFirma { get; set; }
        public string? UbicacionFirma { get; set; }
    }
}
