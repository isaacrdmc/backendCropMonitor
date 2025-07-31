using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CropMonitor.DTOs.Recetas
{
    public class RecetaUpdateDto
    {
        [Required(ErrorMessage = "El nombre de la receta es requerido.")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres.")]
        public string NombreReceta { get; set; } 

        public string Descripcion { get; set; }
        public string Instrucciones { get; set; }

        public List<int> CultivoIDs { get; set; } = new List<int>();
    }
}