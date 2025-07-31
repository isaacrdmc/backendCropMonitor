namespace CropMonitor.DTOs.Sensores
{
    public class SensorInModuloDto
    {
        public int SensorID { get; set; }
        public string TipoSensor { get; set; } // Ej. "Temperatura", "Humedad del suelo"
        public string UnidadMedida { get; set; } // Ej. "°C", "%"
        public decimal? ValorLectura { get; set; } // Puede ser null si no hay lectura reciente
        public string? EstadoRiego { get; set; } // "Activo", "Flujo", "Inactivo"

        // Información del cultivo asociado (si existe)
        public int? CultivoID { get; set; } // Será null si el medidor está "vacío"
        public string? CultivoNombre { get; set; }
        public string? CultivoImagenURL { get; set; }

        // Indica si el sensor está en uso (tiene un cultivo asociado o es un espacio activo)
        public bool EnUso => CultivoID.HasValue;
    }
}