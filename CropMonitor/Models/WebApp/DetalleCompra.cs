using CropMonitor.Models.WebApp;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.WebApp
{
    [Table("DetalleCompras")]
    public class DetalleCompra
    {
        [Key]
        public int DetalleCompraID { get; set; }

        public int? CompraID { get; set; }

        public int? ProductoID { get; set; }

        public int? Cantidad { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? CostoUnitario { get; set; }

        [ForeignKey("CompraID")]
        public Compra Compra { get; set; }

        [ForeignKey("ProductoID")]
        public Producto Producto { get; set; }
    }
}