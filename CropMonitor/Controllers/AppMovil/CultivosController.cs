using CropMonitor.Data; // Tu contexto de base de datos
using CropMonitor.DTOs.Cultivos; // Tus DTOs para Cultivos
using CropMonitor.Models.AppMovil; // Tus modelos de la aplicación móvil
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting; // Para IWebHostEnvironment (acceso a wwwroot)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System; // Para DateTime.Now y Guid
using System.Collections.Generic; // Para List
using System.IO; // Para operaciones de archivo (Path, File, Directory)
using System.Linq; // Para LINQ (Any, Select, ToList)
using System.Security.Claims; // Para acceder a los claims del usuario autenticado
using System.Threading.Tasks; // Para Task

namespace CropMonitor.Controllers.AppMovil
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Este atributo asegura que solo los usuarios autenticados puedan acceder a este controlador
    public class CultivosController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // Se usa para obtener la ruta física de la carpeta wwwroot

        // Constructor: inyecta el contexto de la base de datos y el entorno web
        public CultivosController(CropMonitorDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Obtiene una lista de cultivos, con opciones de filtro por temporada y búsqueda de texto.
        /// Incluye si el cultivo es favorito para el usuario autenticado.
        /// (Puede ser usado para la pantalla principal de "todos los cultivos" o la selección de un nuevo cultivo para un medidor)
        /// </summary>
        /// <param name="temporadaId">ID de la temporada para filtrar (opcional). Ej: 1 para Primavera.</param>
        /// <param name="searchText">Texto para buscar en el nombre o descripción del cultivo (opcional).</param>
        /// <returns>Lista de CultivoListDto.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CultivoListDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Si el usuario no está autenticado
        public async Task<IActionResult> GetCultivos(
            [FromQuery] int? temporadaId,
            [FromQuery] string? searchText)
        {
            // Obtener el UsuarioID del token JWT del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                // Esto no debería ocurrir con [Authorize] pero es una buena práctica de seguridad.
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            // Iniciar la consulta de cultivos
            var query = _context.Cultivos
                // Incluir las relaciones necesarias para filtrar por temporada y obtener sus nombres
                .Include(c => c.CultivosTemporadas)
                    .ThenInclude(ct => ct.Temporada)
                .AsQueryable(); // Convertir a IQueryable para construir la consulta dinámicamente

            // Aplicar filtro por temporada si se proporciona un ID válido
            if (temporadaId.HasValue && temporadaId.Value > 0)
            {
                query = query.Where(c => c.CultivosTemporadas.Any(ct => ct.TemporadaID == temporadaId.Value));
            }

            // Aplicar filtro de búsqueda si se proporciona texto
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                // Buscar en Nombre o Descripción (asegurando que Descripción no sea null)
                query = query.Where(c =>
                    c.Nombre.Contains(searchText) ||
                    c.Descripcion != null && c.Descripcion.Contains(searchText));
            }

            // Ejecutar la consulta y obtener los cultivos
            var cultivos = await query.ToListAsync();

            // Obtener todos los CultivoIDs que el usuario actual tiene marcados como favoritos
            // Esto es más eficiente que hacer una consulta individual para cada cultivo
            var favoritosUsuario = await _context.Favoritos
                                                    .Where(f => f.UsuarioID == userId)
                                                    .Select(f => f.CultivoID)
                                                    .ToListAsync();

            // Mapear los modelos Cultivo a los DTOs de lista
            var cultivoListDtos = cultivos.Select(c => new CultivoListDto
            {
                CultivoID = c.CultivoID,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion, // Usar la descripción existente
                ImagenURL = c.ImagenURL != null ? $"/{c.ImagenURL.Replace("\\", "/")}" : null, // Asegura URL relativa correcta
                EsFavorito = favoritosUsuario.Contains(c.CultivoID), // Verifica si el ID del cultivo está en la lista de favoritos
                Temporadas = c.CultivosTemporadas.Select(ct => ct.Temporada.NombreTemporada).ToList() // Nombres de las temporadas asociadas
            }).ToList();

            return Ok(cultivoListDtos); // Retorna la lista de DTOs con un código 200 OK
        }

        /// <summary>
        /// Obtiene la lista de cultivos marcados como favoritos por el usuario autenticado.
        /// (Pantalla izquierda: Fichas favoritas)
        /// </summary>
        /// <returns>Lista de CultivoListDto (adaptado para favoritos).</returns>
        [HttpGet("Favoritos")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CultivoListDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCultivosFavoritos()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            var cultivosFavoritos = await _context.Favoritos
                                                .Where(f => f.UsuarioID == userId)
                                                .Include(f => f.Cultivo) // Cargar los datos del cultivo
                                                    .ThenInclude(c => c.CultivosTemporadas) // Incluir temporadas
                                                        .ThenInclude(ct => ct.Temporada)
                                                .Select(f => f.Cultivo) // Seleccionar solo el objeto Cultivo
                                                .ToListAsync();

            var favoritoListDtos = cultivosFavoritos.Select(c => new CultivoListDto
            {
                CultivoID = c.CultivoID,
                Nombre = c.Nombre,
                ImagenURL = c.ImagenURL != null ? $"/{c.ImagenURL.Replace("\\", "/")}" : null,
                Descripcion = c.Descripcion, // Usar la descripción existente del cultivo
                EsFavorito = true, // Si está en esta lista, siempre es favorito
                Temporadas = c.CultivosTemporadas.Select(ct => ct.Temporada.NombreTemporada).ToList()
            }).ToList();

            return Ok(favoritoListDtos);
        }


        /// <summary>
        /// Obtiene los detalles de un cultivo específico por su ID.
        /// Incluye si el cultivo es favorito para el usuario autenticado.
        /// (Pantalla derecha: Detalle de cultivo (favorito seleccionado))
        /// </summary>
        /// <param name="id">ID del cultivo.</param>
        /// <returns>CultivoDetailDto.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CultivoDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Si el cultivo no se encuentra
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Si el usuario no está autenticado
        public async Task<IActionResult> GetCultivoById(int id)
        {
            // Obtener el UsuarioID del token JWT del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            // Buscar el cultivo por ID, incluyendo sus relaciones con temporadas
            var cultivo = await _context.Cultivos
                .Include(c => c.CultivosTemporadas)
                    .ThenInclude(ct => ct.Temporada)
                .FirstOrDefaultAsync(c => c.CultivoID == id);

            if (cultivo == null)
            {
                return NotFound($"Cultivo con ID {id} no encontrado."); // Retorna 404 si el cultivo no existe
            }

            // Verificar si este cultivo específico es favorito para el usuario actual
            bool esFavorito = await _context.Favoritos
                                                .AnyAsync(f => f.UsuarioID == userId && f.CultivoID == id);

            // Mapear el modelo Cultivo a un DTO de detalle
            var cultivoDetailDto = new CultivoDetailDto
            {
                CultivoID = cultivo.CultivoID,
                Nombre = cultivo.Nombre,
                Descripcion = cultivo.Descripcion,
                ImagenURL = cultivo.ImagenURL != null ? $"/{cultivo.ImagenURL.Replace("\\", "/")}" : null, // Asegura URL relativa correcta
                RequisitosClima = cultivo.RequisitosClima,
                RequisitosAgua = cultivo.RequisitosAgua,
                RequisitosLuz = cultivo.RequisitosLuz,
                EsFavorito = esFavorito,
                Temporadas = cultivo.CultivosTemporadas.Select(ct => ct.Temporada.NombreTemporada).ToList()
            };

            return Ok(cultivoDetailDto); // Retorna el DTO de detalle con un código 200 OK
        }

        /// <summary>
        /// Marca o desmarca un cultivo como favorito para el usuario autenticado.
        /// (Botón de corazón ❤️ en Pantalla de Detalle de cultivo)
        /// </summary>
        /// <param name="cultivoId">ID del cultivo a marcar/desmarcar.</param>
        /// <returns>Mensaje de confirmación y el nuevo estado de favorito.</returns>
        [HttpPost("{cultivoId}/toggle-favorito")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Para cuando ya es favorito y se intenta añadir de nuevo (opcional, el código actual lo manejaría)
        public async Task<IActionResult> ToggleFavorito(int cultivoId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            var cultivo = await _context.Cultivos.FindAsync(cultivoId);
            if (cultivo == null)
            {
                return NotFound($"Cultivo con ID {cultivoId} no encontrado.");
            }

            // Buscar si ya existe una entrada de favorito para este usuario y cultivo
            var favoritoExistente = await _context.Favoritos
                                                .FirstOrDefaultAsync(f => f.UsuarioID == userId && f.CultivoID == cultivoId);

            bool esFavoritoAhora;

            if (favoritoExistente != null)
            {
                // Ya es favorito, desmarcar (eliminar de la tabla Favoritos)
                _context.Favoritos.Remove(favoritoExistente);
                esFavoritoAhora = false;
            }
            else
            {
                // No es favorito, marcar (añadir a la tabla Favoritos)
                var nuevoFavorito = new Favorito
                {
                    UsuarioID = userId,
                    CultivoID = cultivoId,
                    FechaAgregado = DateTime.Now // Asignar la fecha actual
                };
                _context.Favoritos.Add(nuevoFavorito);
                esFavoritoAhora = true;
            }

            await _context.SaveChangesAsync(); // Guardar los cambios en la base de datos

            return Ok(new { Message = $"Cultivo {cultivo.Nombre} actualizado como favorito.", EsFavorito = esFavoritoAhora });
        }


        /// <summary>
        /// Crea un nuevo cultivo y sube su imagen asociada.
        /// </summary>
        /// <param name="cultivoDto">Datos del cultivo y el archivo de imagen. Se envía como FormData.</param>
        /// <returns>El cultivo creado con sus detalles.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // Cuando se crea un recurso exitosamente
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Si los datos de entrada son inválidos
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Si el usuario no está autenticado
        // Usamos [FromForm] porque la solicitud contiene tanto datos de formulario como un archivo (IFormFile)
        public async Task<IActionResult> CreateCultivo([FromForm] CultivoCreateUpdateDto cultivoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna errores de validación del DTO
            }

            string imageUrl = null;
            if (cultivoDto.ImagenFile != null)
            {
                // Llama al método auxiliar para guardar la imagen y obtener su URL
                imageUrl = await SaveImage(cultivoDto.ImagenFile);
                if (imageUrl == null)
                {
                    // Manejo de error si la imagen no se pudo guardar
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error al guardar la imagen.");
                }
            }

            // Crea una nueva instancia del modelo Cultivo
            var cultivo = new Cultivo
            {
                Nombre = cultivoDto.Nombre,
                Descripcion = cultivoDto.Descripcion,
                ImagenURL = imageUrl, // Asigna la URL generada a la propiedad ImagenURL
                RequisitosClima = cultivoDto.RequisitosClima,
                RequisitosAgua = cultivoDto.RequisitosAgua,
                RequisitosLuz = cultivoDto.RequisitosLuz
            };

            // Asignar temporadas si se proporcionaron IDs de temporada
            if (cultivoDto.TemporadaIDs != null && cultivoDto.TemporadaIDs.Any())
            {
                // Para evitar errores si la colección es nula al inicio
                if (cultivo.CultivosTemporadas == null)
                {
                    cultivo.CultivosTemporadas = new List<CultivosTemporada>();
                }
                foreach (var temporadaId in cultivoDto.TemporadaIDs)
                {
                    // Buscar la temporada para asegurar que existe
                    var temporada = await _context.Temporadas.FindAsync(temporadaId);
                    if (temporada != null)
                    {
                        cultivo.CultivosTemporadas.Add(new CultivosTemporada
                        {
                            Cultivo = cultivo, // Asocia el cultivo que se está creando
                            Temporada = temporada
                        });
                    }
                    else
                    {
                        ModelState.AddModelError("TemporadaIDs", $"La Temporada con ID {temporadaId} no existe.");
                        return BadRequest(ModelState);
                    }
                }
            }

            _context.Cultivos.Add(cultivo); // Agrega el nuevo cultivo al contexto
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

            // Obtener el UsuarioID del token JWT para el DTO de respuesta (aunque un nuevo cultivo no es favorito)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = 0;
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
            {
                userId = parsedUserId;
            }

            // Preparar el DTO de respuesta (similar a GetCultivoById)
            bool esFavorito = false; // Un cultivo recién creado no será favorito por defecto para el usuario
            var temporadasAsociadas = cultivo.CultivosTemporadas?.Select(ct => ct.Temporada.NombreTemporada).ToList() ?? new List<string>();

            var createdCultivoDto = new CultivoDetailDto
            {
                CultivoID = cultivo.CultivoID,
                Nombre = cultivo.Nombre,
                Descripcion = cultivo.Descripcion,
                ImagenURL = cultivo.ImagenURL != null ? $"/{cultivo.ImagenURL.Replace("\\", "/")}" : null,
                RequisitosClima = cultivo.RequisitosClima,
                RequisitosAgua = cultivo.RequisitosAgua,
                RequisitosLuz = cultivo.RequisitosLuz,
                EsFavorito = esFavorito,
                Temporadas = temporadasAsociadas
            };

            // Retorna 201 Created, con la URL para acceder al recurso recién creado y el DTO
            return CreatedAtAction(nameof(GetCultivoById), new { id = cultivo.CultivoID }, createdCultivoDto);
        }

        /// <summary>
        /// Actualiza un cultivo existente por su ID, incluyendo la opción de cambiar su imagen.
        /// </summary>
        /// <param name="id">ID del cultivo a actualizar.</param>
        /// <param name="cultivoDto">Nuevos datos del cultivo y el archivo de imagen (opcional). Se envía como FormData.</param>
        /// <returns>Estado de la operación de actualización.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Cuando la actualización es exitosa
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Si los datos de entrada son inválidos
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Si el cultivo no se encuentra
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Si el usuario no está autenticado
        public async Task<IActionResult> UpdateCultivo(int id, [FromForm] CultivoCreateUpdateDto cultivoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Buscar el cultivo, incluyendo sus temporadas para poder actualizarlas
            var cultivo = await _context.Cultivos
                                        .Include(c => c.CultivosTemporadas)
                                        .FirstOrDefaultAsync(c => c.CultivoID == id);

            if (cultivo == null)
            {
                return NotFound($"Cultivo con ID {id} no encontrado.");
            }

            // Actualizar propiedades básicas del cultivo
            cultivo.Nombre = cultivoDto.Nombre;
            cultivo.Descripcion = cultivoDto.Descripcion;
            cultivo.RequisitosClima = cultivoDto.RequisitosClima;
            cultivo.RequisitosAgua = cultivoDto.RequisitosAgua;
            cultivo.RequisitosLuz = cultivoDto.RequisitosLuz;

            // Manejo de la imagen si se proporciona una nueva
            if (cultivoDto.ImagenFile != null)
            {
                // Si ya existía una imagen, la eliminamos del servidor
                if (!string.IsNullOrEmpty(cultivo.ImagenURL))
                {
                    DeleteImage(cultivo.ImagenURL); // Llama al método auxiliar para borrar
                }
                // Guarda la nueva imagen y actualiza la URL en el modelo
                cultivo.ImagenURL = await SaveImage(cultivoDto.ImagenFile);
                if (cultivo.ImagenURL == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error al guardar la nueva imagen.");
                }
            }
            // Si ImagenFile es null, no se cambia la URL de la imagen existente.
            // Si el cliente quiere quitar la imagen sin reemplazarla, debería enviar un string vacío o un valor específico
            // que tu frontend interprete para borrar, y aquí necesitarías una lógica adicional para setear ImagenURL a null
            // y borrar la imagen existente. Por simplicidad, este ejemplo solo reemplaza o mantiene.

            // Lógica para actualizar las temporadas asociadas (si se proporcionan nuevos IDs)
            if (cultivoDto.TemporadaIDs != null)
            {
                // Limpiar las asociaciones de temporadas existentes
                _context.Cultivos_Temporadas.RemoveRange(cultivo.CultivosTemporadas);

                // Añadir las nuevas asociaciones de temporadas
                foreach (var temporadaId in cultivoDto.TemporadaIDs)
                {
                    var temporada = await _context.Temporadas.FindAsync(temporadaId);
                    if (temporada != null)
                    {
                        cultivo.CultivosTemporadas.Add(new CultivosTemporada { CultivoID = cultivo.CultivoID, TemporadaID = temporada.TemporadaID });
                    }
                    else
                    {
                        ModelState.AddModelError("TemporadaIDs", $"La Temporada con ID {temporadaId} no existe.");
                        return BadRequest(ModelState);
                    }
                }
            }
            else
            {
                // Si TemporadaIDs es null, podríamos optar por no cambiar las temporadas o borrar todas.
                // Aquí, si es null, las temporadas existentes se mantendrían si no se quitó la línea RemoveRange.
                // Con RemoveRange + Add, si TemporadaIDs es null, simplemente se borrarían todas las asociaciones.
                // Considera la lógica de tu frontend: si no envía TemporadaIDs, ¿significa que no hay cambios o que se borran todas?
                // Si el propósito es mantener las existentes, no deberías llamar RemoveRange si cultivoDto.TemporadaIDs es null.
            }

            // Marcar el cultivo como modificado para que EF Core sepa que debe actualizarlo
            _context.Entry(cultivo).State = EntityState.Modified;

            // Para asegurar que los cambios en CultivosTemporada se persistan, si los hubo
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Cultivo con ID {id} actualizado exitosamente." }); // Retorna 200 OK
        }

        /// <summary>
        /// Elimina un cultivo por su ID.
        /// También elimina la imagen asociada del servidor.
        /// </summary>
        /// <param name="id">ID del cultivo a eliminar.</param>
        /// <returns>Estado de la operación de eliminación.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Cuando la eliminación es exitosa (no hay contenido para retornar)
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Si el cultivo no se encuentra
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Si el usuario no está autenticado
        public async Task<IActionResult> DeleteCultivo(int id)
        {
            var cultivo = await _context.Cultivos.FindAsync(id); // Buscar el cultivo por ID
            if (cultivo == null)
            {
                return NotFound($"Cultivo con ID {id} no encontrado."); // Retorna 404 si no existe
            }

            // Eliminar imagen asociada del servidor si existe
            if (!string.IsNullOrEmpty(cultivo.ImagenURL))
            {
                DeleteImage(cultivo.ImagenURL); // Llama al método auxiliar para borrar el archivo físico
            }

            _context.Cultivos.Remove(cultivo); // Marca el cultivo para ser eliminado
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos

            return NoContent(); // Retorna 204 No Content (éxito sin cuerpo de respuesta)
        }


        // =========================================================
        // MÉTODOS AUXILIARES PARA EL MANEJO DE ARCHIVOS DE IMAGEN
        // =========================================================

        /// <summary>
        /// Guarda un archivo de imagen en el servidor y retorna su URL relativa.
        /// </summary>
        /// <param name="imageFile">El archivo de imagen a guardar.</param>
        /// <returns>URL relativa de la imagen o null si hay un error.</returns>
        private async Task<string> SaveImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null; // No hay archivo o está vacío
            }

            // Define la ruta física donde se guardarán las imágenes dentro de wwwroot
            // Por ejemplo: C:\TuProyecto\wwwroot\images\cultivos\
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "cultivos");

            // Crea la carpeta si no existe
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Genera un nombre de archivo único para evitar colisiones
            // Usa Guid.NewGuid() para asegurar unicidad y Path.GetExtension para mantener la extensión original
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Guarda el archivo físico en la ruta definida
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream); // Copia el contenido del archivo subido al disco
            }

            // Retorna la URL relativa que se almacenará en la base de datos y será accesible desde el cliente
            // Ej: "images/cultivos/abcdef12345.jpg"
            // Se usa Replace("\\", "/") para asegurar que la URL sea compatible con web (Linux/Windows)
            return Path.Combine("images", "cultivos", uniqueFileName).Replace("\\", "/");
        }

        /// <summary>
        /// Elimina un archivo de imagen del servidor dada su URL relativa.
        /// </summary>
        /// <param name="imageUrl">La URL relativa de la imagen a eliminar (ej: "images/cultivos/archivo.jpg").</param>
        private void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return; // No hay URL o es vacía, no hay nada que borrar

            // Extrae solo el nombre del archivo de la URL relativa
            var fileName = Path.GetFileName(imageUrl.Replace("/", "\\")); // Asegura formato de ruta Windows para Path.GetFileName

            // Construye la ruta física completa del archivo
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "cultivos", fileName);

            // Verifica si el archivo existe antes de intentar eliminarlo
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath); // Elimina el archivo
            }
        }
    }
}