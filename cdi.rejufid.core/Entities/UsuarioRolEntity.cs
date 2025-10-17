namespace cdi.rejufid.core.Entities
{
    public class UsuarioRolEntity
    {
        public int Id_usuario_roles { get; set; }
        public string? Id_usuario { get; set; }
        public string? Nombre_completo { get; set; }
        public string? Matricula { get; set; }
        public string? Correo { get; set; }
        public int Id_rol { get; set; }
        public DateTime Fecha_asignacion { get; set; }
        public string? Contrasena { get; set; } 
    }
}
