using CropMonitor.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models
{
    public class Planta
    {
        [Key]
        public int IdPlanta { get; set; }

        [Required]
        [StringLength(100)]
        public string NombrePlanta { get; set; }

        [StringLength(100)]
        public string TipoPlanta { get; set; }

        [StringLength(100)]
        public string UsoPlanta { get; set; }

        [StringLength(255)]
        public string FotoPlanta { get; set; }

        public bool Favorito { get; set; } = false;

        public int? IdModulo { get; set; }
        [ForeignKey("IdModulo")]
        public Modulo Modulo { get; set; }

        // Optional: If you decide to explicitly link Guia
        // public int? IdGuia { get; set; }
        // [ForeignKey("IdGuia")]
        // public Guia Guia { get; set; }
    }
}