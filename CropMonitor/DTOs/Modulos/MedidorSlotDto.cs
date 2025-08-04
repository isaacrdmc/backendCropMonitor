namespace CropMonitor.DTOs.Modulos
{
    // Este DTO representa uno de los 4 espacios de cultivo dentro de un módulo.
    public class MedidorSlotDto
    {
        public int MedidorSlotIndex { get; set; }
        public bool EnUso { get; set; } // Indica si el slot tiene un cultivo asignado

        // Información del cultivo, si está asignado
        public int? CultivoID { get; set; }
        public string? CultivoNombre { get; set; }
        public string? CultivoImagenURL { get; set; }

        // Lista de los sensores que pertenecen a este slot
        public List<SensorLecturaDto> Sensores { get; set; } = new List<SensorLecturaDto>();
    }

    // Un DTO más simple para la información del sensor dentro del slot
    public class SensorLecturaDto
    {
        public int SensorID { get; set; }
        public string TipoSensor { get; set; }
        public string UnidadMedida { get; set; }
        public decimal? ValorLectura { get; set; }
        public string? EstadoRiego { get; set; }
    }
}