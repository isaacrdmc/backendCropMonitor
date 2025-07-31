using CropMonitor.Models.AppMovil;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.AppMovil
{
    [Table("Sensores")]
    public class Sensor
    {
        [Key]
        public int SensorID { get; set; }

        [Required]
        public int ModuloID { get; set; }

        public int? CultivoID { get; set; } // Puede ser NULL si el espacio está vacío

        [Required]
        [StringLength(50)]
        public string TipoSensor { get; set; } // Ej: 'Temperatura', 'Humedad del suelo', 'pH', 'Luz'

        [Required] // Asegura que cada sensor pertenece a un slot
        public int MedidorSlotIndex { get; set; } // Identifica a cuál de los 4 "medidores" pertenece (0-3)

        [StringLength(10)]
        public string UnidadMedida { get; set; } // Ej: '°C', '%', 'pH', 'lux'

        public DateTime? UltimaLectura { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? ValorLectura { get; set; }

        [StringLength(50)]
        public string EstadoRiego { get; set; } // Ej: 'Activo', 'Flujo', 'Inactivo'

        public bool EsAcuaHidroponico { get; set; } = false;

        [ForeignKey("ModuloID")]
        public Modulo Modulo { get; set; }

        [ForeignKey("CultivoID")]
        public Cultivo Cultivo { get; set; } // Clave foránea anulable

        // Propiedades de navegación
        public ICollection<LecturaSensor> LecturasSensores { get; set; }
        public ICollection<Notificacion> Notificaciones { get; set; }
    }
}