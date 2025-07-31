// CropMonitor.Controllers/RecetasController.cs
using CropMonitor.Data;
using CropMonitor.DTOs.Recetas;
using CropMonitor.Models.AppMovil;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CropMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize] // Considera si solo usuarios autenticados pueden ver o gestionar recetas
    public class RecetasController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;

        public RecetasController(CropMonitorDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene una lista de todas las recetas.
        /// </summary>
        /// <returns>Lista de RecetaListDto.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RecetaListDto>))]
        public async Task<ActionResult<IEnumerable<RecetaListDto>>> GetRecetas()
        {
            var recetas = await _context.Recetas
                .Select(r => new RecetaListDto
                {
                    RecetaID = r.RecetaID,
                    NombreReceta = r.NombreReceta, // ¡CAMBIO AQUÍ!
                    DescripcionCorta = r.Descripcion != null && r.Descripcion.Length > 150 ? r.Descripcion.Substring(0, 150) + "..." : r.Descripcion
                })
                .ToListAsync();

            return Ok(recetas);
        }

        /// <summary>
        /// Obtiene los detalles de una receta específica por ID.
        /// </summary>
        /// <param name="id">ID de la receta.</param>
        /// <returns>RecetaDetailDto.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecetaDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RecetaDetailDto>> GetReceta(int id)
        {
            var receta = await _context.Recetas
                .Include(r => r.RecetasCultivos)
                .ThenInclude(rc => rc.Cultivo)
                .FirstOrDefaultAsync(r => r.RecetaID == id);

            if (receta == null)
            {
                return NotFound("Receta no encontrada.");
            }

            var recetaDto = new RecetaDetailDto
            {
                RecetaID = receta.RecetaID,
                NombreReceta = receta.NombreReceta, // ¡CAMBIO AQUÍ!
                Descripcion = receta.Descripcion,
                Instrucciones = receta.Instrucciones,
                CultivosNecesarios = receta.RecetasCultivos
                    .Select(rc => new CultivoEnRecetaDto
                    {
                        CultivoID = rc.Cultivo.CultivoID,
                        NombreCultivo = rc.Cultivo.Nombre // Asegúrate de que tu modelo Cultivo tiene una propiedad NombreCultivo
                    })
                    .ToList()
            };

            return Ok(recetaDto);
        }

        /// <summary>
        /// Busca recetas por nombre o por el nombre de un cultivo requerido.
        /// </summary>
        /// <param name="query">Término de búsqueda (nombre de receta o nombre de cultivo).</param>
        /// <returns>Lista de RecetaListDto.</returns>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<RecetaListDto>))]
        public async Task<ActionResult<IEnumerable<RecetaListDto>>> SearchRecetas([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                // Si la consulta está vacía, puedes devolver todas las recetas o un error BadRequest.
                // Aquí, devolveremos todas las recetas, similar al GetRecetas sin filtro.
                return await GetRecetas();
            }

            var lowerQuery = query.ToLower();

            var recetas = await _context.Recetas
                .Include(r => r.RecetasCultivos)
                .ThenInclude(rc => rc.Cultivo)
                .Where(r => r.NombreReceta.ToLower().Contains(lowerQuery) || // ¡CAMBIO AQUÍ!
                            r.RecetasCultivos.Any(rc => rc.Cultivo.Nombre.ToLower().Contains(lowerQuery)))
                .Select(r => new RecetaListDto
                {
                    RecetaID = r.RecetaID,
                    NombreReceta = r.NombreReceta, // ¡CAMBIO AQUÍ!
                    DescripcionCorta = r.Descripcion != null && r.Descripcion.Length > 150 ? r.Descripcion.Substring(0, 150) + "..." : r.Descripcion
                })
                .ToListAsync();

            return Ok(recetas);
        }


        /// <summary>
        /// Crea una nueva receta.
        /// </summary>
        /// <param name="recetaDto">Datos de la nueva receta.</param>
        /// <returns>La receta creada.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RecetaDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [Authorize(Roles = "Admin")] // Si solo los administradores pueden crear recetas
        public async Task<ActionResult<RecetaDetailDto>> CreateReceta([FromBody] RecetaCreateDto recetaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newReceta = new Receta
            {
                NombreReceta = recetaDto.NombreReceta, // ¡CAMBIO AQUÍ!
                Descripcion = recetaDto.Descripcion,
                Instrucciones = recetaDto.Instrucciones,
                RecetasCultivos = new List<RecetasCultivo>() // Inicializar la colección
            };

            // Añadir relación con cultivos
            if (recetaDto.CultivoIDs != null && recetaDto.CultivoIDs.Any())
            {
                foreach (var cultivoId in recetaDto.CultivoIDs.Distinct())
                {
                    var cultivoExists = await _context.Cultivos.AnyAsync(c => c.CultivoID == cultivoId);
                    if (!cultivoExists)
                    {
                        return BadRequest($"El CultivoID {cultivoId} no existe.");
                    }
                    newReceta.RecetasCultivos.Add(new RecetasCultivo { CultivoID = cultivoId });
                }
            }

            _context.Recetas.Add(newReceta);
            await _context.SaveChangesAsync();

            // Retornar el detalle de la receta creada (incluyendo cultivos)
            var createdReceta = await _context.Recetas
                .Include(r => r.RecetasCultivos)
                .ThenInclude(rc => rc.Cultivo)
                .FirstOrDefaultAsync(r => r.RecetaID == newReceta.RecetaID);

            var createdRecetaDto = new RecetaDetailDto
            {
                RecetaID = createdReceta.RecetaID,
                NombreReceta = createdReceta.NombreReceta, // ¡CAMBIO AQUÍ!
                Descripcion = createdReceta.Descripcion,
                Instrucciones = createdReceta.Instrucciones,
                CultivosNecesarios = createdReceta.RecetasCultivos
                    .Select(rc => new CultivoEnRecetaDto
                    {
                        CultivoID = rc.Cultivo.CultivoID,
                        NombreCultivo = rc.Cultivo.Nombre
                    })
                    .ToList()
            };

            return CreatedAtAction(nameof(GetReceta), new { id = newReceta.RecetaID }, createdRecetaDto);
        }

        /// <summary>
        /// Actualiza una receta existente por ID.
        /// </summary>
        /// <param name="id">ID de la receta a actualizar.</param>
        /// <param name="recetaDto">Datos actualizados de la receta.</param>
        /// <returns>Estado de la operación.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateReceta(int id, [FromBody] RecetaUpdateDto recetaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var recetaToUpdate = await _context.Recetas
                .Include(r => r.RecetasCultivos) // Incluir la tabla de unión para poder modificarla
                .FirstOrDefaultAsync(r => r.RecetaID == id);

            if (recetaToUpdate == null)
            {
                return NotFound("Receta no encontrada.");
            }

            recetaToUpdate.NombreReceta = recetaDto.NombreReceta; // ¡CAMBIO AQUÍ!
            recetaToUpdate.Descripcion = recetaDto.Descripcion;
            recetaToUpdate.Instrucciones = recetaDto.Instrucciones;

            // Actualizar relaciones con cultivos
            if (recetaDto.CultivoIDs != null)
            {
                // Eliminar relaciones antiguas
                _context.Recetas_Cultivos.RemoveRange(recetaToUpdate.RecetasCultivos); // ¡CAMBIO AQUÍ: Usar el DbSet correcto!

                // Añadir nuevas relaciones
                foreach (var cultivoId in recetaDto.CultivoIDs.Distinct())
                {
                    var cultivoExists = await _context.Cultivos.AnyAsync(c => c.CultivoID == cultivoId);
                    if (!cultivoExists)
                    {
                        return BadRequest($"El CultivoID {cultivoId} no existe.");
                    }
                    recetaToUpdate.RecetasCultivos.Add(new RecetasCultivo { RecetaID = id, CultivoID = cultivoId });
                }
            }
            else
            {
                // Si CultivoIDs es null, significa que no se desea modificar los cultivos.
                // Si se desea borrarlos, el cliente debe enviar una lista vacía.
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecetaExists(id))
                {
                    return NotFound("Receta no encontrada.");
                }
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Elimina una receta por ID.
        /// </summary>
        /// <param name="id">ID de la receta a eliminar.</param>
        /// <returns>Estado de la operación.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReceta(int id)
        {
            var receta = await _context.Recetas.FindAsync(id);
            if (receta == null)
            {
                return NotFound("Receta no encontrada.");
            }

            _context.Recetas.Remove(receta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecetaExists(int id)
        {
            return _context.Recetas.Any(e => e.RecetaID == id);
        }
    }
}