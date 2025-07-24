using CropMonitor.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models
{
    public class Comentario
    {
        [Key]
        public int IdComentario { get; set; }

        [StringLength(255)]
        public string Titulo { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string ComentarioTexto { get; set; } // Renamed to avoid conflict with class name

        public int MeGusta { get; set; } = 0;

        public DateTime FechaComentario { get; set; } = DateTime.Now;

        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }
    }
}