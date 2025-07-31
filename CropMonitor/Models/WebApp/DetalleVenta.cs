using CropMonitor.Models.WebApp;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.WebApp
{
    [Table("DetalleVentas")]
    public class DetalleVenta
    {
        [Key]
        public int DetalleVentaID { get; set; }

        public int? VentaID { get; set; }

        public int? ProductoID { get; set; }

        public int? Cantidad { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? PrecioVenta { get; set; }

        [ForeignKey("VentaID")]
        public Venta Venta { get; set; }

        [ForeignKey("ProductoID")]
        public Producto Producto { get; set; }
    }
}