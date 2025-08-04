using CropMonitor.Data;
using CropMonitor.DTOs.Usuarios;
using CropMonitor.Models.AppMovil; // Asegúrate de que este using apunte a tu modelo Usuario
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Para PasswordHasher
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CropMonitor.Controllers.AppMovil
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Asegura que solo usuarios autenticados puedan acceder
    public class UsuariosController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher; // Para hashear y verificar contraseñas

        public UsuariosController(CropMonitorDbContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher; // Asegúrate de registrar IPasswordHasher en Startup.cs
        }

        /// <summary>
        /// Obtiene el perfil del usuario autenticado.
        /// (Para mostrar Correo electrónico y Nombre de usuario)
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsuarioProfileDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // En caso de que el usuario no se encuentre (raro con Authorize)
        public async Task<IActionResult> GetCurrentUserProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            var usuario = await _context.Usuarios.FindAsync(userId);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var userProfileDto = new UsuarioProfileDto
            {
                UsuarioID = usuario.UsuarioID,
                NombreUsuario = usuario.Nombre, // Asume que tienes esta propiedad en tu modelo Usuario
                Correo = usuario.Correo
                // Si tuvieras un campo de Google Login en Usuario, lo asignarías aquí
                // RegistradoConGoogle = usuario.RegistradoConGoogle
            };

            return Ok(userProfileDto);
        }

        /// <summary>
        /// Permite al usuario autenticado cambiar su contraseña.
        /// (Para la sección de Contraseña)
        /// </summary>
        /// <param name="changePasswordDto">DTO con la contraseña actual y la nueva contraseña.</param>
        [HttpPut("me/password")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Verificar la contraseña actual
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(usuario, usuario.ContrasenaHash, changePasswordDto.CurrentPassword);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return BadRequest("La contraseña actual es incorrecta.");
            }

            // Hashear la nueva contraseña y actualizar
            usuario.ContrasenaHash = _passwordHasher.HashPassword(usuario, changePasswordDto.NewPassword);

            await _context.SaveChangesAsync();

            return Ok("Contraseña actualizada exitosamente.");
        }

        /// <summary>
        /// Permite al usuario autenticado eliminar su cuenta.
        /// (Para el botón "Eliminar cuenta")
        /// </summary>
        /// <remarks>
        /// **¡ADVERTENCIA!** Eliminar un usuario puede tener implicaciones en cascada.
        /// Asegúrate de que las relaciones en tu base de datos están configuradas con `OnDelete`
        /// apropiado (CASCADE, SET NULL, o RESTRICT y manejar manualmente)
        /// para todos los datos relacionados (Favoritos, Modulos, LecturasSensores, Notificaciones, etc.).
        /// Considera implementar una "eliminación suave" (soft delete) marcando el usuario como inactivo
        /// en lugar de una eliminación física, si la integridad de los datos históricos es importante.
        /// </remarks>
        [HttpDelete("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))] // En caso de errores por restricciones de FK
        public async Task<IActionResult> DeleteAccount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Entity Framework Core, al detectar el OnDelete(DeleteBehavior.Cascade) configurado
            // en OnModelCreating y aplicado a la BD, se encargará de eliminar los registros relacionados.
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync(); // Esto es lo que activa la eliminación en cascada en la DB

            return NoContent();
        }
    }
}