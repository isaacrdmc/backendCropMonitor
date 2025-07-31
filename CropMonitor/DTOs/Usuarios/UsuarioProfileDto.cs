namespace CropMonitor.DTOs.Usuarios
{
    public class UsuarioProfileDto
    {
        public int UsuarioID { get; set; }
        public string NombreUsuario { get; set; } // O el nombre que uses para el login/display
        public string Correo { get; set; }
    }
}