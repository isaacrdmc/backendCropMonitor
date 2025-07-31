using CropMonitor.Models.WebApp;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.WebApp
{
    [Table("Kardex")]
    public class Kardex
    {
        [Key]
        public int KardexID { get; set; }

        public int? ProductoID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Fecha { get; set; }

        [StringLength(50)]
        public string TipoMovimiento { get; set; }

        public int? Cantidad { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? CostoUnitario { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Promedio { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Debe { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Haber { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Saldo { get; set; }

        [ForeignKey("ProductoID")]
        public Producto Producto { get; set; }
    }
}