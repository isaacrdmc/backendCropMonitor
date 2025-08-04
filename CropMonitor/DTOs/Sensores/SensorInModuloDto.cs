namespace CropMonitor.DTOs.Sensores
{
    public class SensorInModuloDto
    {
        public int SensorID { get; set; }
        public string TipoSensor { get; set; } // Ej. "Temperatura", "Humedad del suelo"
        public string UnidadMedida { get; set; } // Ej. "°C", "%"
        public decimal? ValorLectura { get; set; } // Puede ser null si no hay lectura reciente
        public string? EstadoRiego { get; set; } // "Activo", "Flujo", "Inactivo"


    }
}