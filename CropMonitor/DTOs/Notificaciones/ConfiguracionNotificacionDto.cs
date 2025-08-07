namespace CropMonitor.DTOs.Notificaciones
{
    public class ConfiguracionNotificacionDto
    {
        public string FrecuenciaRiego { get; set; }
        public TimeSpan? HorarioNotificacion { get; set; }
        public bool ActivarRiegoAutomatico { get; set; }
        public string TipoAlertaSensor { get; set; }
        public bool HabilitarRecomendacionesEstacionales { get; set; }
    }
}
