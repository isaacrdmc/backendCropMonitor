using CropMonitor.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.WebApp
{
    [Table("Ventas")]
    public class Venta
    {
        [Key]
        public int VentaID { get; set; }

        public int? ClienteID { get; set; }

        public DateTime FechaVenta { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Total { get; set; }

        [ForeignKey("ClienteID")]
        public Cliente Cliente { get; set; }

        // Propiedades de navegación
        public ICollection<DetalleVenta> DetalleVentas { get; set; }
    }
}