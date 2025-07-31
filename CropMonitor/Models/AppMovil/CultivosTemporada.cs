using CropMonitor.Models.AppMovil;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.AppMovil
{
    [Table("Cultivos_Temporadas")]
    public class CultivosTemporada
    {
        [Key]
        [Column(Order = 0)] // Parte de la clave primaria compuesta
        public int CultivoID { get; set; }

        [Key]
        [Column(Order = 1)] // Parte de la clave primaria compuesta
        public int TemporadaID { get; set; }

        [ForeignKey("CultivoID")]
        public Cultivo Cultivo { get; set; }

        [ForeignKey("TemporadaID")]
        public Temporada Temporada { get; set; }
    }
}