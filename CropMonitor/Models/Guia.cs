using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models
{
    public class Guia
    {
        [Key]
        public int IdGuia { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoPlanta { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string CuidadoPlanta { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string NotasPlantas { get; set; }

        [StringLength(50)]
        public string EstacionPlanta { get; set; }
    }
}