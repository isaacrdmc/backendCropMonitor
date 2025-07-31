using CropMonitor.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.WebApp
{
    [Table("Productos")]
    public class Producto
    {
        [Key]
        public int ProductoID { get; set; }

        [StringLength(255)]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Precio { get; set; }

        public string ImagenURL { get; set; }

        public int? Stock { get; set; }

        [StringLength(50)]
        public string Unidad { get; set; }

        // Propiedades de navegación
        public ICollection<DetalleCompra> DetalleCompras { get; set; }
        public ICollection<DetalleVenta> DetalleVentas { get; set; }
        public ICollection<Kardex> Kardex { get; set; }
    }
}