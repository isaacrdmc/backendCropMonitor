using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace CropMonitor.Models
{
    public class Modulo
    {
        [Key]
        public int IdModulo { get; set; }

        [Required]
        [StringLength(50)]
        public string CodigoModulo { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoModulo { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Costo { get; set; }

        public VentaModulo VentaModulo { get; set; } // One-to-one with VentaModulo
        public Planta Planta { get; set; } // One-to-one with Planta
        public ICollection<Estadistica> Estadisticas { get; set; }
    }
}