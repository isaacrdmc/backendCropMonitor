using System.ComponentModel.DataAnnotations;

namespace CropMonitor.DTOs.Usuarios
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "La contraseña actual es requerida.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La nueva contraseña debe tener entre 6 y 100 caracteres.")]
        // Puedes añadir más validaciones de complejidad aquí, como caracteres especiales, números, etc.
        // [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{6,}$", ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula, un número y un carácter especial.")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "La confirmación de la nueva contraseña no coincide.")]
        public string ConfirmNewPassword { get; set; }
    }
}