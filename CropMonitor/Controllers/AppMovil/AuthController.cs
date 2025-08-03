using BCrypt.Net; // Asegúrate de tener BCrypt.Net-Next instalado vía NuGet
using CropMonitor.Data;
using CropMonitor.DTOs.Auth;
using CropMonitor.Models.AppMovil;
using CropMonitorApi.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic; // Necesario para List<Claim>
using Microsoft.Extensions.Configuration;

namespace CropMonitor.Controllers.AppMovil
{
    [Route("api/mobile/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher; // <-- Declarado aquí
        private readonly IConfiguration _configuration;

        // ** MODIFICACIÓN AQUÍ: Asegúrate de que el constructor recibe passwordHasher **
        public AuthController(CropMonitorDbContext context, IPasswordHasher<Usuario> passwordHasher, IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher; // <-- ¡Aquí es donde se asigna!
            _configuration = configuration;
        }

        /// <summary>
        /// Registra un nuevo usuario en la aplicación.
        /// </summary>
        /// <param name="registerRequest">Datos de registro del usuario (Nombre, Correo, Contraseña).</param>
        /// <returns>Estado de la operación de registro.</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verificar si el correo ya está registrado
            if (await _context.Usuarios.AnyAsync(u => u.Correo == registerRequest.Correo))
            {
                return BadRequest("El correo electrónico ya está registrado.");
            }

            var newUser = new Usuario
            {
                Nombre = registerRequest.Nombre,
                Correo = registerRequest.Correo,
                TipoUsuario = "Casual",
                RolUsuario = "Cliente",
                EmailConfirmado = false
            };

            // --- CAMBIO CLAVE AQUÍ ---
            // Hashear la contraseña usando IPasswordHasher (de ASP.NET Core Identity)
            newUser.ContrasenaHash = _passwordHasher.HashPassword(newUser, registerRequest.Contrasena); // <--- Usa el PasswordHasher inyectado
                                                                                                        // --- FIN CAMBIO CLAVE ---

            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuario registrado exitosamente." });
        }

        /// <summary>
        /// Autentica a un usuario y devuelve un token JWT.
        /// </summary>
        /// <param name="loginRequest">Credenciales del usuario (Correo, Contraseña).</param>
        /// <returns>Token JWT y datos básicos del usuario.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == loginRequest.Correo);

            if (user == null)
            {
                return Unauthorized(new { Message = "Credenciales inválidas. Correo o contraseña incorrectos." });
            }

            // --- CAMBIO CLAVE AQUÍ ---
            // Verificar la contraseña hasheada usando IPasswordHasher
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.ContrasenaHash, loginRequest.Contrasena);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { Message = "Credenciales inválidas. Correo o contraseña incorrectos." });
            }
            // --- FIN CAMBIO CLAVE ---

            // Generar Token JWT (el resto del código es el mismo)
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UsuarioID.ToString()), // ID del usuario
            new Claim(ClaimTypes.Email, user.Correo),
            new Claim(ClaimTypes.Name, user.Nombre),
            new Claim(ClaimTypes.Role, user.RolUsuario ?? "Usuario"), // Rol del usuario
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID único del token
        };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(24), // Token válido por 24 horas
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new LoginResponseDto
            {
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                UsuarioID = user.UsuarioID,
                Correo = user.Correo,
                Nombre = user.Nombre
            });
        }
    }
}