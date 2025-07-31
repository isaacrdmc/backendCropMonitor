using CropMonitor.Models.AppMovil;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.AppMovil
{
    [Table("Notificaciones")]
    public class Notificacion
    {
        [Key]
        public int NotificacionID { get; set; }

        [Required]
        public int UsuarioID { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoNotificacion { get; set; } // Ej: 'Riego', 'pH Crítico', 'Alerta Sensor'

        [Required]
        public string Mensaje { get; set; }

        public DateTime FechaHoraEnvio { get; set; } = DateTime.Now;

        public bool Leida { get; set; } = false;

        public int? CultivoID { get; set; } // Opcional
        public int? SensorID { get; set; } // Opcional

        [ForeignKey("UsuarioID")]
        public Usuario Usuario { get; set; }

        [ForeignKey("CultivoID")]
        public Cultivo Cultivo { get; set; } // Clave foránea anulable

        [ForeignKey("SensorID")]
        public Sensor Sensor { get; set; } // Clave foránea anulable
    }
}