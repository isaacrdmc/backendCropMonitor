using CropMonitor.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.WebApp
{
    [Table("Compras")]
    public class Compra
    {
        [Key]
        public int CompraID { get; set; }

        public int? ProveedorID { get; set; }

        public DateTime FechaCompra { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Total { get; set; }

        [ForeignKey("ProveedorID")]
        public Proveedor Proveedor { get; set; }

        // Propiedades de navegación
        public ICollection<DetalleCompra> DetalleCompras { get; set; }
    }
}