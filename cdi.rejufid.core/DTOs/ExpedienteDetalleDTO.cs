namespace cdi.rejufid.core.DTOs
{
    public class ExpedienteDetalleDTO
    {
        public int Id_expediente { get; set; }
        public int Id_estado { get; set; }
        public int Id_tipo_organo { get; set; }
        public int Id_materia { get; set; }
        public int Id_organo { get; set; }
        public int Id_tipo_asunto { get; set; }
        public string Numero_expediente { get; set; }
        public int Anio_expediente { get; set; }
        public DateTime Fecha_expediente { get; set; }
        public string Observacion { get; set; }
        public int Id_estatus { get; set; }
        public string Usuario_registro { get; set; }
        public DateTime Fecha_registro { get; set; }

        // Nombres descriptivos para mostrar en el frontend
        public string Estado { get; set; }
        public string Tipo_organo { get; set; }
        public string Materia { get; set; }
        public string Organo { get; set; }
        public string Tipo_asunto { get; set; }
        public string Estatus { get; set; }
    }
}