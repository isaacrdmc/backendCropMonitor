using CropMonitor.Data;
using CropMonitor.Models.AppMovil;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CropMonitor.WebApp.Controllers
{
    [Route("api/web/[controller]")]
    [ApiController]
    public class AuthWebController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly CropMonitorDbContext _context;

        public AuthWebController(IConfiguration configuration, CropMonitorDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAdmin(string correo, string contrasena)
        {
            try
            {
                // Validaciones básicas
                if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
                {
                    return BadRequest(new { message = "Correo y contraseña son requeridos" });
                }

                // Buscar usuario por correo
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Correo == correo);

                if (usuario == null)
                {
                    return Unauthorized(new { message = "Credenciales inválidas" });
                }

                // Verificar contraseña (implementa tu lógica real de verificación)
                if (!VerifyPasswordHash(contrasena, usuario.ContrasenaHash))
                {
                    return Unauthorized(new { message = "Credenciales inválidas" });
                }

                // Verificar que sea administrador
                if (usuario.TipoUsuario != "Admin")
                {
                    return Forbid(); // 403 - Prohibido (no es admin)
                }

                // Verificar si el email está confirmado
                if (!usuario.EmailConfirmado)
                {
                    return Unauthorized(new { message = "Por favor confirma tu email antes de iniciar sesión" });
                }

                // Generar token JWT
                var token = GenerateJwtToken(usuario);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    usuario = new
                    {
                        usuario.UsuarioID,
                        usuario.Nombre,
                        usuario.Correo,
                        usuario.TipoUsuario,
                        usuario.RolUsuario
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            // Implementa tu lógica real de verificación de hash aquí
            // EJEMPLO SIMPLIFICADO (NO USAR EN PRODUCCIÓN):
            return password == storedHash;
        }

        private JwtSecurityToken GenerateJwtToken(Usuario usuario)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Correo),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.TipoUsuario),
                new Claim("RolUsuario", usuario.RolUsuario)
            };

            return new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: credentials);
        }
    }
}