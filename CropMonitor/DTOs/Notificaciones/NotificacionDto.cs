namespace CropMonitor.DTOs.Notificaciones
{
    // DTO para la lista de notificaciones que se envían al cliente.
    // Incluye información del cultivo o sensor para una mejor visualización.
    public class NotificacionDto
    {
        public int NotificacionID { get; set; }
        public string TipoNotificacion { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaHoraEnvio { get; set; }
        public bool Leida { get; set; }
        public int? CultivoID { get; set; }
        public string CultivoNombre { get; set; } // Nombre del cultivo, si aplica
        public int? SensorID { get; set; }
        public string TipoSensor { get; set; } // Nombre del sensor, si aplica
    }
}
