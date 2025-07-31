namespace CropMonitor.DTOs.Sensores
{
    public class SensorDetailDto
    {
        public int SensorID { get; set; }
        public string ModuloNombre { get; set; } // Nombre del módulo al que pertenece
        public string TipoSensor { get; set; }
        public string UnidadMedida { get; set; }
        public decimal? ValorLectura { get; set; }
        public DateTime? UltimaLectura { get; set; }
        public string? EstadoRiego { get; set; } //
        public bool EsAcuaHidroponico { get; set; } //

        // Información del cultivo asociado
        public int? CultivoID { get; set; }
        public string? CultivoNombre { get; set; }
        public string? CultivoImagenURL { get; set; }
        public string? CultivoRequisitosClima { get; set; } //
        public string? CultivoRequisitosAgua { get; set; } //
        public string? CultivoRequisitosLuz { get; set; } //

        // Tips específicos para este cultivo
        public List<string> TipsParaEstaPlanta { get; set; } = new List<string>();
    }
}