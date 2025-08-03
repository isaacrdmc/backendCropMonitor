using CropMonitor.Data;
using CropMonitor.Models.AppMovil;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CropMonitor.Controllers.WebApp
{
    [Route("api/web/[controller]")]
    [ApiController]
    [Authorize(Roles = "AdminWeb")]
    public class UsuariosController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuariosController(CropMonitorDbContext context, IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: api/web/Usuarios?search=texto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios([FromQuery] string? search)
        {
            IQueryable<Usuario> query = _context.Usuarios;

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    u.Nombre.Contains(search) ||
                    u.Correo.Contains(search));
            }

            return await query.ToListAsync();
        }

        // POST: api/web/Usuarios/registrar-admin
        [HttpPost("registrar-admin")]
        public async Task<ActionResult<Usuario>> RegistrarAdmin([FromBody] UsuarioRegistroDto registroDto)
        {
            var usuario = new Usuario
            {
                Nombre = registroDto.Nombre,
                Correo = registroDto.Correo,
                RolUsuario = "AdminWeb",
                EmailConfirmado = true
            };

            usuario.ContrasenaHash = _passwordHasher.HashPassword(usuario, registroDto.Contrasena);
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuarios), usuario);
        }

        // PUT: api/web/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, UsuarioUpdateDto updateDto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.Nombre = updateDto.Nombre ?? usuario.Nombre;
            usuario.RolUsuario = updateDto.Rol ?? usuario.RolUsuario;

            if (!string.IsNullOrEmpty(updateDto.Contrasena))
            {
                usuario.ContrasenaHash = _passwordHasher.HashPassword(usuario, updateDto.Contrasena);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    public class UsuarioRegistroDto
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
    }

    public class UsuarioUpdateDto
    {
        public string? Nombre { get; set; }
        public string? Rol { get; set; }
        public string? Contrasena { get; set; }
    }
}