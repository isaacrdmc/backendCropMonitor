using CropMonitor.Models.AppMovil;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.AppMovil
{
    [Table("Recetas")]
    public class Receta
    {
        [Key]
        public int RecetaID { get; set; }

        [Required]
        [StringLength(255)]
        public string NombreReceta { get; set; }

        public string Descripcion { get; set; }

        public string Instrucciones { get; set; }

        // Propiedades de navegación
        public ICollection<RecetasCultivo> RecetasCultivos { get; set; }
    }
}