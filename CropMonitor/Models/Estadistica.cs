using CropMonitor.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models
{
    public class Estadistica
    {
        [Key]
        public int IdEstadistica { get; set; }

        public int IdModulo { get; set; }
        [ForeignKey("IdModulo")]
        public Modulo Modulo { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? ValorTemperatura { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? ValorHumedad { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal? ValorLuz { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? NivelAgua { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;
    }
}