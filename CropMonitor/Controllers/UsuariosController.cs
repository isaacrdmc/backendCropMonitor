using BCrypt.Net; // For password hashing if updating password
using CropMonitor.Data;
using CropMonitor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CropMonitor.Controllers
{
    [Authorize] // Secure the Users controller
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly CropMonitorContext _context;

        public UsuariosController(CropMonitorContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        [Authorize(Roles = "Administrador")] // Only Admins can list all users
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Propietario,Regular")] // Admins can see any, users can see their own
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            // For a regular user, ensure they can only access their own profile
            if (User.IsInRole("Regular") || User.IsInRole("Propietario"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || int.Parse(userIdClaim) != id)
                {
                    return Forbid("You are not authorized to access this user's data.");
                }
            }

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            // Do not return hashed password in API response
            usuario.Contrasena = null;
            return usuario;
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Propietario,Regular")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return BadRequest();
            }

            // For a regular user, ensure they can only update their own profile
            if (User.IsInRole("Regular") || User.IsInRole("Propietario"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || int.Parse(userIdClaim) != id)
                {
                    return Forbid("You are not authorized to update this user's data.");
                }
            }

            var existingUser = await _context.Usuarios.AsNoTracking().SingleOrDefaultAsync(u => u.IdUsuario == id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Only update fields that are allowed for modification
            existingUser.Nombre = usuario.Nombre;
            existingUser.Correo = usuario.Correo;
            // Only update password if provided and different
            if (!string.IsNullOrEmpty(usuario.Contrasena) && !BCrypt.Net.BCrypt.Verify(usuario.Contrasena, existingUser.Contrasena))
            {
                existingUser.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
            }

            // Admin can update Type and Role
            if (User.IsInRole("Administrador"))
            {
                existingUser.Tipo = usuario.Tipo;
                existingUser.Rol = usuario.Rol;
            }

            _context.Entry(existingUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Usuarios
        [HttpPost]
        [AllowAnonymous] // Allow public registration, or [Authorize(Roles = "Administrador")] if only admin can create
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo))
            {
                return Conflict("User with this email already exists.");
            }

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Do not return hashed password
            usuario.Contrasena = null;
            return CreatedAtAction("GetUsuario", new { id = usuario.IdUsuario }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")] // Only Admins can delete users
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}