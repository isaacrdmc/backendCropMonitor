using CropMonitor.Data;
using CropMonitor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CropMonitor.Controllers
{
    [Authorize] // Secure the Guias controller
    [Route("api/[controller]")]
    [ApiController]
    public class GuiasController : ControllerBase
    {
        private readonly CropMonitorContext _context;

        public GuiasController(CropMonitorContext context)
        {
            _context = context;
        }

        // GET: api/Guias
        [HttpGet]
        [AllowAnonymous] // Guides are generally public information
        public async Task<ActionResult<IEnumerable<Guia>>> GetGuias()
        {
            return await _context.Guias.ToListAsync();
        }

        // GET: api/Guias/5
        [HttpGet("{id}")]
        [AllowAnonymous] // Guides are generally public information
        public async Task<ActionResult<Guia>> GetGuia(int id)
        {
            var guia = await _context.Guias.FindAsync(id);

            if (guia == null)
            {
                return NotFound();
            }

            return guia;
        }

        // PUT: api/Guias/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")] // Only Admins can update guides
        public async Task<IActionResult> PutGuia(int id, Guia guia)
        {
            if (id != guia.IdGuia)
            {
                return BadRequest();
            }

            _context.Entry(guia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GuiaExists(id))
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

        // POST: api/Guias
        [HttpPost]
        [Authorize(Roles = "Administrador")] // Only Admins can create guides
        public async Task<ActionResult<Guia>> PostGuia(Guia guia)
        {
            _context.Guias.Add(guia);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGuia", new { id = guia.IdGuia }, guia);
        }

        // DELETE: api/Guias/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")] // Only Admins can delete guides
        public async Task<IActionResult> DeleteGuia(int id)
        {
            var guia = await _context.Guias.FindAsync(id);
            if (guia == null)
            {
                return NotFound();
            }

            _context.Guias.Remove(guia);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GuiaExists(int id)
        {
            return _context.Guias.Any(e => e.IdGuia == id);
        }
    }
}