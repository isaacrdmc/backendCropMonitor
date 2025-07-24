using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Correo { get; set; }

        [Required]
        [StringLength(255)]
        public string Contrasena { get; set; } // Hashed password

        [StringLength(50)]
        public string Tipo { get; set; } // e.g., 'Administrador', 'Regular'

        [StringLength(50)]
        public string Rol { get; set; } // e.g., 'Propietario', 'Miembro'

        public ICollection<VentaModulo> VentaModulos { get; set; }
        public ICollection<Comentario> Comentarios { get; set; }
    }
}