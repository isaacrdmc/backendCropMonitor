using CropMonitor.Models.AppMovil;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.AppMovil
{
    [Table("ConfiguracionNotificaciones")]
    public class ConfiguracionNotificacion
    {
        [Key]
        public int ConfiguracionID { get; set; }

        [Required]
        public int UsuarioID { get; set; }

        [StringLength(50)]
        public string FrecuenciaRiego { get; set; } // Ej: 'Diario', 'Semanal'

        [Column(TypeName = "time")]
        public TimeSpan? HorarioNotificacion { get; set; }

        public bool ActivarRiegoAutomatico { get; set; } = false;

        [StringLength(50)]
        public string TipoAlertaSensor { get; set; } // Ej: 'Email', 'Notificación en app'

        public bool HabilitarRecomendacionesEstacionales { get; set; } = true;

        [ForeignKey("UsuarioID")]
        public Usuario Usuario { get; set; }
    }
}