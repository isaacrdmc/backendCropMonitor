using CropMonitor.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models
{
    public class Sensor
    {
        [Key]
        public int IdSensor { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreSensor { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoSensor { get; set; } // e.g., 'Temperatura', 'Humedad', 'Luz', 'NivelAgua'

        [StringLength(255)]
        public string Descripcion { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? RangoMin { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? RangoMax { get; set; }

        [Required]
        public int IdModulo { get; set; } // Each sensor belongs to a module
        [ForeignKey("IdModulo")]
        public Modulo Modulo { get; set; }
    }
}