namespace CropMonitorApi.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string JwtToken { get; set; }
        public int UsuarioID { get; set; }
        public string Correo { get; set; }
        public string Nombre { get; set; }
    }
}