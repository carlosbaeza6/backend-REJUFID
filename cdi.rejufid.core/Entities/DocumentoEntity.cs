namespace cdi.rejufid.core.DTOs;
public class DocumentoEntity
{
    public int Id_documento { get; set; }
    public int Id_expediente { get; set; }
    public int Id_tipo_archivo { get; set; } 
    public string Nombre_archivo { get; set; }
    public string Ruta_archivo { get; set; }
    public bool Esta_firmado { get; set; }
    public DateTime Fecha_carga { get; set; }
    public string Usuario_carga { get; set; }
    public string? Usuario_firma { get; set; }
    public DateTime? Fecha_firma { get; set; }
    public string? Hash_documento { get; set; }
    public string? Uuid_firma { get; set; }
    public string? Firma_base64 { get; set; }
}
