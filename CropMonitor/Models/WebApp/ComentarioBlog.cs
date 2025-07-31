using CropMonitor.Models.AppMovil;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.WebApp
{
    [Table("ComentariosBlog")]
    public class ComentarioBlog
    {
        [Key]
        public int ComentarioID { get; set; }

        [Required]
        public int BlogID { get; set; }

        public int? UsuarioID { get; set; } // Puede ser NULL si un usuario no autenticado comenta

        public string? Comentario { get; set; }

        public DateTime FechaComentario { get; set; } = DateTime.Now;

        [ForeignKey("BlogID")]
        public Blog Blog { get; set; }

        [ForeignKey("UsuarioID")]
        public Usuario Usuario { get; set; } // Clave foránea anulable
    }
}