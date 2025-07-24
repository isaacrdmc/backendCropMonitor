using CropMonitor.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models
{
    public class VentaModulo
    {
        [Key]
        public int IdVentas { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.Now;

        public int CantidadVenta { get; set; } = 1;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalVenta { get; set; }

        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }

        public int IdModulo { get; set; }
        [ForeignKey("IdModulo")]
        public Modulo Modulo { get; set; }
    }
}