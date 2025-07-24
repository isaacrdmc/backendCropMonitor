using CropMonitor.Data;
using CropMonitor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CropMonitor.Controllers
{
    [Authorize] // Secure the Plantas controller
    [Route("api/[controller]")]
    [ApiController]
    public class PlantasController : ControllerBase
    {
        private readonly CropMonitorContext _context;

        public PlantasController(CropMonitorContext context)
        {
            _context = context;
        }

        // GET: api/Plantas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Planta>>> GetPlantas()
        {
            // Users should typically only see plants they own (via Modulo ownership)
            // or plants that are public if such a concept exists.
            // For this example, if a user is not an Admin, they will only see plants linked to modules they've "bought".
            if (User.IsInRole("Administrador"))
            {
                return await _context.Plantas.Include(p => p.Modulo).ToListAsync();
            }
            else
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var userPlantas = await _context.Plantas
                    .Include(p => p.Modulo)
                    .Where(p => p.Modulo != null && _context.VentaModulos.Any(vm => vm.IdModulo == p.Modulo.IdModulo && vm.IdUsuario == userId))
                    .ToListAsync();

                return userPlantas;
            }
        }

        // GET: api/Plantas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Planta>> GetPlanta(int id)
        {
            var planta = await _context.Plantas.Include(p => p.Modulo).FirstOrDefaultAsync(p => p.IdPlanta == id);

            if (planta == null)
            {
                return NotFound();
            }

            // Authorization check: Ensure the user owns this plant's module or is an admin.
            if (!User.IsInRole("Administrador"))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (planta.IdModulo == null || !_context.VentaModulos.Any(vm => vm.IdModulo == planta.IdModulo && vm.IdUsuario == userId))
                {
                    return Forbid("You are not authorized to view this plant.");
                }
            }

            return planta;
        }

        // PUT: api/Plantas/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Propietario")] // Admins and Owners can update plants
        public async Task<IActionResult> PutPlanta(int id, Planta planta)
        {
            if (id != planta.IdPlanta)
            {
                return BadRequest();
            }

            // Authorization check: Ensure the user owns this plant's module or is an admin.
            if (!User.IsInRole("Administrador"))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (planta.IdModulo == null || !_context.VentaModulos.Any(vm => vm.IdModulo == planta.IdModulo && vm.IdUsuario == userId))
                {
                    return Forbid("You are not authorized to update this plant.");
                }
            }

            _context.Entry(planta).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantaExists(id))
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

        // POST: api/Plantas
        [HttpPost]
        [Authorize(Roles = "Administrador,Propietario")] // Admins and Owners can create plants
        public async Task<ActionResult<Planta>> PostPlanta(Planta planta)
        {
            // Validate Modulo existence and ensure it's not already assigned to another plant (IdModulo is UNIQUE NULL)
            if (planta.IdModulo.HasValue)
            {
                var modulo = await _context.Modulos.FindAsync(planta.IdModulo.Value);
                if (modulo == null)
                {
                    return BadRequest("The specified Modulo does not exist.");
                }
                var existingPlantaWithModule = await _context.Plantas.AnyAsync(p => p.IdModulo == planta.IdModulo.Value);
                if (existingPlantaWithModule)
                {
                    return BadRequest("The specified Modulo is already assigned to another plant.");
                }

                // If 'Propietario', ensure the module belongs to them
                if (!User.IsInRole("Administrador"))
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    var isOwnedModule = await _context.VentaModulos.AnyAsync(vm => vm.IdModulo == planta.IdModulo.Value && vm.IdUsuario == userId);
                    if (!isOwnedModule)
                    {
                        return Forbid("You can only assign modules that you own.");
                    }
                }
            }

            _context.Plantas.Add(planta);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlanta", new { id = planta.IdPlanta }, planta);
        }

        // DELETE: api/Plantas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Propietario")] // Admins and Owners can delete plants
        public async Task<IActionResult> DeletePlanta(int id)
        {
            var planta = await _context.Plantas.FindAsync(id);
            if (planta == null)
            {
                return NotFound();
            }

            // Authorization check: Ensure the user owns this plant's module or is an admin.
            if (!User.IsInRole("Administrador"))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (planta.IdModulo == null || !_context.VentaModulos.Any(vm => vm.IdModulo == planta.IdModulo && vm.IdUsuario == userId))
                {
                    return Forbid("You are not authorized to delete this plant.");
                }
            }

            _context.Plantas.Remove(planta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlantaExists(int id)
        {
            return _context.Plantas.Any(e => e.IdPlanta == id);
        }
    }
}