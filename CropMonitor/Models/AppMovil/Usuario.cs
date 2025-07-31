using CropMonitor.Models.AppMovil;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // Para [JsonIgnore]

namespace CropMonitor.Models.AppMovil
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        public int UsuarioID { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(255)]
        public string Correo { get; set; }

        [Required]
        [StringLength(255)]
        [JsonIgnore] // Excluir de la serialización JSON por seguridad
        public string ContrasenaHash { get; set; }

        [StringLength(50)]
        public string TipoUsuario { get; set; } // Ej: 'Admin', 'Estandar'

        [StringLength(50)]
        public string RolUsuario { get; set; } // Ej: 'Agricultor', 'Espectador'

        public bool EmailConfirmado { get; set; } = false;

        // Propiedades de navegación
        public ICollection<Modulo> Modulos { get; set; }
        public ICollection<TipsCultivo> TipsCultivos { get; set; }
        public ICollection<Favorito> Favoritos { get; set; }
        public ICollection<Notificacion> Notificaciones { get; set; }
        public ConfiguracionNotificacion ConfiguracionNotificaciones { get; set; }
        // Se relaciona con ComentarioBlog de la sección WebApp
        public ICollection<CropMonitor.Models.WebApp.ComentarioBlog> ComentariosBlog { get; set; }
    }
}