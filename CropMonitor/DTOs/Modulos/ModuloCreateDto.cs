using System.ComponentModel.DataAnnotations;

namespace CropMonitor.DTOs.Modulos
{
    public class ModuloCreateDto
    {
        [Required(ErrorMessage = "El nombre del módulo es obligatorio.")]
        [StringLength(255)]
        public string NombreModulo { get; set; }

        // Estado inicial, por ejemplo "Inactivo" o "Activo" por defecto
        public string Estado { get; set; } = "Activo"; // Puedes poner un valor por defecto

        // Días en funcionamiento iniciales
        public int DiasEnFuncionamiento { get; set; } = 0;

        // Cantidad de cultivos actual se inicializará a 0
        // No se necesita enviar el UsuarioID desde el cliente, se tomará del token JWT.
    }
}