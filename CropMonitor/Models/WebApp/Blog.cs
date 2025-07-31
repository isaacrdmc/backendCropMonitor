using CropMonitor.Models.WebApp;
using System;
using System.Collections.Generic; // Asegúrate de tener este using
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.WebApp
{
    [Table("Blog")]
    public class Blog
    {
        [Key]
        public int BlogID { get; set; }

        [Required]
        [StringLength(255)]
        public string Titulo { get; set; }

        [Required]
        public string Contenido { get; set; }

        public string ImagenURL { get; set; }

        public DateTime FechaPublicacion { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string Autor { get; set; }

        // Propiedad de navegación CORRECTA para la colección de comentarios
        public ICollection<ComentarioBlog> ComentariosBlog { get; set; }

        // REMUEVE ESTA LÍNEA si no la necesitas para otra lógica NO relacionada con EF Core:
        // public object Comentario { get; internal set; }
    }
}