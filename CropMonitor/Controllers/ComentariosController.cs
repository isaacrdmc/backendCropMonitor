
using CropMonitor.Data;
using CropMonitor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CropMonitor.Controllers
{
    [Authorize] // Secure the Comentarios controller
    [Route("api/[controller]")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly CropMonitorContext _context;

        public ComentariosController(CropMonitorContext context)
        {
            _context = context;
        }

        // GET: api/Comentarios
        [HttpGet]
        [AllowAnonymous] // Comments might be public, or restrict to logged-in users.
                         // Keeping it public for general forums/discussions.
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentarios()
        {
            return await _context.Comentarios.Include(c => c.Usuario).ToListAsync();
        }

        // GET: api/Comentarios/5
        [HttpGet("{id}")]
        [AllowAnonymous] // Public comment view
        public async Task<ActionResult<Comentario>> GetComentario(int id)
        {
            var comentario = await _context.Comentarios.Include(c => c.Usuario).FirstOrDefaultAsync(c => c.IdComentario == id);

            if (comentario == null)
            {
                return NotFound();
            }

            return comentario;
        }

        // PUT: api/Comentarios/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,Regular,Propietario")] // Admins can edit any, regular users/owners can edit their own
        public async Task<IActionResult> PutComentario(int id, Comentario comentario)
        {
            if (id != comentario.IdComentario)
            {
                return BadRequest();
            }

            // Authorization check: User can only update their own comment, unless they are an Admin.
            if (!User.IsInRole("Administrador"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || int.Parse(userIdClaim) != comentario.IdUsuario)
                {
                    return Forbid("You are not authorized to update this comment.");
                }
            }

            // Ensure the IdUsuario is not changed by the user in the request body
            var existingComment = await _context.Comentarios.AsNoTracking().FirstOrDefaultAsync(c => c.IdComentario == id);
            if (existingComment == null)
            {
                return NotFound();
            }

            // Keep the original IdUsuario from the database
            comentario.IdUsuario = existingComment.IdUsuario;
            comentario.FechaComentario = existingComment.FechaComentario; // Maintain original creation date

            _context.Entry(comentario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComentarioExists(id))
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

        // POST: api/Comentarios
        [HttpPost]
        [Authorize(Roles = "Regular,Propietario,Administrador")] // Logged-in users can create comments
        public async Task<ActionResult<Comentario>> PostComentario(Comentario comentario)
        {
            // Set the IdUsuario from the authenticated user's claim
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated."); // Should not happen with [Authorize]
            }
            comentario.IdUsuario = int.Parse(userIdClaim);

            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComentario", new { id = comentario.IdComentario }, comentario);
        }

        // DELETE: api/Comentarios/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador,Regular,Propietario")] // Admins can delete any, regular users/owners can delete their own
        public async Task<IActionResult> DeleteComentario(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario == null)
            {
                return NotFound();
            }

            // Authorization check: User can only delete their own comment, unless they are an Admin.
            if (!User.IsInRole("Administrador"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || int.Parse(userIdClaim) != comentario.IdUsuario)
                {
                    return Forbid("You are not authorized to delete this comment.");
                }
            }

            _context.Comentarios.Remove(comentario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Endpoint to increment 'MeGusta' (Likes)
        [HttpPost("{id}/like")]
        [Authorize] // Any logged-in user can like a comment
        public async Task<IActionResult> LikeComentario(int id)
        {
            var comentario = await _context.Comentarios.FindAsync(id);
            if (comentario == null)
            {
                return NotFound();
            }

            comentario.MeGusta++;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Comment liked successfully.", Likes = comentario.MeGusta });
        }


        private bool ComentarioExists(int id)
        {
            return _context.Comentarios.Any(e => e.IdComentario == id);
        }
    }
}