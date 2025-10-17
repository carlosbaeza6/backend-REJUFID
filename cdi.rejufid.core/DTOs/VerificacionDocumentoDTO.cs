namespace cdi.rejufid.core.DTOs
{
    public class VerificacionDocumentoDTO
    {
        public bool Valido { get; set; }
        public string? Hash { get; set; }
        public string? Uuid { get; set; }
        public int? IdDocumento { get; set; }
        public int? IdExpediente { get; set; }
        public string? UsuarioFirmante { get; set; }
        public DateTime? FechaFirma { get; set; }
        public string? Mensaje { get; set; }
    }
}
