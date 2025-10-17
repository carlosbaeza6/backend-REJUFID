namespace cdi.rejufid.core.DTOs
{
    public class FirmaRespuestaDTO
    {
        public byte[] DocumentoFirmado { get; set; } = default!;
        public string Hash { get; set; } = default!;
        public string Uuid { get; set; } = default!;
        public string UsuarioFirmante { get; set; } = default!;
        public string FirmaBase64 { get; set; } = default!;
    }
}

