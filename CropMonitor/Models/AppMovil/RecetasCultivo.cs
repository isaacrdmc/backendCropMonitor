using CropMonitor.Models.AppMovil;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.AppMovil
{
    [Table("Recetas_Cultivos")]
    public class RecetasCultivo
    {
        [Key]
        [Column(Order = 0)] // Parte de la clave primaria compuesta
        public int RecetaID { get; set; }

        [Key]
        [Column(Order = 1)] // Parte de la clave primaria compuesta
        public int CultivoID { get; set; }

        [ForeignKey("RecetaID")]
        public Receta Receta { get; set; }

        [ForeignKey("CultivoID")]
        public Cultivo Cultivo { get; set; }
    }
}