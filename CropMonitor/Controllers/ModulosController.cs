using CropMonitor.Data;
using CropMonitor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CropMonitor.Controllers
{
    [Authorize] // Secure the Modulos controller
    [Route("api/[controller]")]
    [ApiController]
    public class ModulosController : ControllerBase
    {
        private readonly CropMonitorContext _context;

        public ModulosController(CropMonitorContext context)
        {
            _context = context;
        }

        // GET: api/Modulos
        [HttpGet]
        [Authorize(Roles = "Administrador,Propietario")] // Admins can see all, Owners can see their own
        public async Task<ActionResult<IEnumerable<Modulo>>> GetModulos()
        {
            // For a 'Propietario' role, you might want to filter modules owned by the current user.
            // This would involve joining with VentaModulo and checking the UserId.
            // For simplicity, this example returns all modules for authorized users.
            return await _context.Modulos.ToListAsync();
        }

        // GET: api/Modulos/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrador,Propietario")]
        public async Task<ActionResult<Modulo>> GetModulo(int id)
        {
            var modulo = await _context.Modulos.FindAsync(id);

            if (modulo == null)
            {
                return NotFound();
            }

            // Optional: If 'Propietario' can only see their own modules, add a check here.
            // You'd need to fetch VentaModulo to determine ownership.

            return modulo;
        }

        // PUT: api/Modulos/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")] // Only Admins can update modules
        public async Task<IActionResult> PutModulo(int id, Modulo modulo)
        {
            if (id != modulo.IdModulo)
            {
                return BadRequest();
            }

            _context.Entry(modulo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuloExists(id))
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

        // POST: api/Modulos
        [HttpPost]
        [Authorize(Roles = "Administrador")] // Only Admins can create new module types
        public async Task<ActionResult<Modulo>> PostModulo(Modulo modulo)
        {
            _context.Modulos.Add(modulo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModulo", new { id = modulo.IdModulo }, modulo);
        }

        // DELETE: api/Modulos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")] // Only Admins can delete modules
        public async Task<IActionResult> DeleteModulo(int id)
        {
            var modulo = await _context.Modulos.FindAsync(id);
            if (modulo == null)
            {
                return NotFound();
            }

            _context.Modulos.Remove(modulo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ModuloExists(int id)
        {
            return _context.Modulos.Any(e => e.IdModulo == id);
        }
    }
}