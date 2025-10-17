namespace cdi.rejufid.core.DTOs
{
    public class DocumentoRespuestaDTO
    {
        public int Id_documento { get; set; }
        public int Id_expediente { get; set; }
        public int Id_tipo_archivo { get; set; }
        public string Nombre_archivo { get; set; }
        public string Ruta_archivo { get; set; }
        public bool Esta_firmado { get; set; }
        public DateTime Fecha_carga { get; set; }
        public string Usuario_carga { get; set; }
    }
}