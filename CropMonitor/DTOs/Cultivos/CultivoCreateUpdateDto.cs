using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Para IFormFile

namespace CropMonitor.DTOs.Cultivos
{
    public class CultivoCreateUpdateDto
    {
        [Required(ErrorMessage = "El nombre del cultivo es obligatorio.")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres.")]
        public string Nombre { get; set; }

        public string? Descripcion { get; set; }

        // La imagen se envía como un archivo, no como una URL aquí.
        // La URL se generará en el servidor después de guardar la imagen.
        public IFormFile? ImagenFile { get; set; }

        [StringLength(255)]
        public string? RequisitosClima { get; set; }

        [StringLength(255)]
        public string? RequisitosAgua { get; set; }

        [StringLength(255)]
        public string? RequisitosLuz { get; set; }

        // Aquí podrías incluir los IDs de las temporadas si se asignan al crear
        public List<int>? TemporadaIDs { get; set; }
    }
}