namespace cdi.rejufid.core.Entities
{
    public class ExpedienteEntity
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
    }
}