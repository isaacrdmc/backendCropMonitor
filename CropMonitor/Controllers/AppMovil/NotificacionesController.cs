using CropMonitor.Data; // Tu contexto de base de datos
using CropMonitor.DTOs.Notificaciones;
using CropMonitor.Models.AppMovil; // Tus modelos de la aplicación móvil
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CropMonitor.Controllers.AppMovil
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Este atributo asegura que solo los usuarios autenticados puedan acceder a este controlador
    public class NotificacionesController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;

        // Constructor: inyecta el contexto de la base de datos
        public NotificacionesController(CropMonitorDbContext context)
        {
            _context = context;
        }

        // Método auxiliar para obtener el ID de usuario del token JWT
        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("No se pudo identificar al usuario autenticado.");
            }
            return userId;
        }

        /// <summary>
        /// Obtiene todas las notificaciones para el usuario autenticado, ordenadas por fecha descendente.
        /// </summary>
        /// <param name="leida">Opcional. Filtra por notificaciones leídas o no leídas.</param>
        /// <returns>Lista de NotificacionDto.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<NotificacionDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetNotificaciones([FromQuery] bool? leida = null)
        {
            try
            {
                var userId = GetUserIdFromToken();

                var query = _context.Notificaciones
                    .Where(n => n.UsuarioID == userId)
                    .Include(n => n.Cultivo)
                    .Include(n => n.Sensor)
                    .AsQueryable();

                // Aplicar filtro si se especifica un valor
                if (leida.HasValue)
                {
                    query = query.Where(n => n.Leida == leida.Value);
                }

                var notificaciones = await query
                    .OrderByDescending(n => n.FechaHoraEnvio)
                    .Select(n => new NotificacionDto
                    {
                        NotificacionID = n.NotificacionID,
                        TipoNotificacion = n.TipoNotificacion,
                        Mensaje = n.Mensaje,
                        FechaHoraEnvio = n.FechaHoraEnvio,
                        Leida = n.Leida,
                        CultivoID = n.CultivoID,
                        CultivoNombre = n.Cultivo.Nombre,
                        SensorID = n.SensorID,
                        TipoSensor = n.Sensor.TipoSensor
                    })
                    .ToListAsync();

                return Ok(notificaciones);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene la cantidad de notificaciones no leídas para el usuario autenticado.
        /// </summary>
        /// <returns>Un objeto con la cantidad.</returns>
        [HttpGet("cantidad-no-leidas")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCantidadNotificacionesNoLeidas()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var cantidad = await _context.Notificaciones
                    .CountAsync(n => n.UsuarioID == userId && !n.Leida);

                return Ok(new { Cantidad = cantidad });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Marca una notificación como leída.
        /// </summary>
        /// <param name="notificacionId">El ID de la notificación a marcar.</param>
        [HttpPost("{notificacionId}/marcar-como-leida")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarcarComoLeida(int notificacionId)
        {
            try
            {
                var userId = GetUserIdFromToken();

                var notificacion = await _context.Notificaciones
                    .FirstOrDefaultAsync(n => n.NotificacionID == notificacionId && n.UsuarioID == userId);

                if (notificacion == null)
                {
                    return NotFound("Notificación no encontrada.");
                }

                notificacion.Leida = true;
                _context.Notificaciones.Update(notificacion);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Notificación marcada como leída exitosamente." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene la configuración de notificaciones del usuario autenticado.
        /// </summary>
        /// <returns>ConfiguracionNotificacionDto.</returns>
        [HttpGet("configuracion")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConfiguracionNotificacionDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetConfiguracionNotificaciones()
        {
            try
            {
                var userId = GetUserIdFromToken();

                var configuracion = await _context.ConfiguracionNotificaciones
                    .FirstOrDefaultAsync(c => c.UsuarioID == userId);

                // Si no hay configuración, se devuelve una configuración por defecto
                if (configuracion == null)
                {
                    return Ok(new ConfiguracionNotificacionDto());
                }

                var dto = new ConfiguracionNotificacionDto
                {
                    FrecuenciaRiego = configuracion.FrecuenciaRiego,
                    HorarioNotificacion = configuracion.HorarioNotificacion,
                    ActivarRiegoAutomatico = configuracion.ActivarRiegoAutomatico,
                    TipoAlertaSensor = configuracion.TipoAlertaSensor,
                    HabilitarRecomendacionesEstacionales = configuracion.HabilitarRecomendacionesEstacionales
                };

                return Ok(dto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        /// <summary>
        /// Actualiza la configuración de notificaciones del usuario autenticado.
        /// Si no existe una configuración, se crea una nueva.
        /// </summary>
        /// <param name="configDto">El DTO con la configuración a actualizar.</param>
        /// <returns>La configuración actualizada.</returns>
        [HttpPut("configuracion")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConfiguracionNotificacionDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateConfiguracionNotificaciones(ConfiguracionNotificacionDto configDto)
        {
            try
            {
                var userId = GetUserIdFromToken();

                var configuracion = await _context.ConfiguracionNotificaciones
                    .FirstOrDefaultAsync(c => c.UsuarioID == userId);

                if (configuracion == null)
                {
                    // Si no existe, crear un nuevo registro
                    configuracion = new ConfiguracionNotificacion
                    {
                        UsuarioID = userId,
                        FrecuenciaRiego = configDto.FrecuenciaRiego,
                        HorarioNotificacion = configDto.HorarioNotificacion,
                        ActivarRiegoAutomatico = configDto.ActivarRiegoAutomatico,
                        TipoAlertaSensor = configDto.TipoAlertaSensor,
                        HabilitarRecomendacionesEstacionales = configDto.HabilitarRecomendacionesEstacionales
                    };
                    _context.ConfiguracionNotificaciones.Add(configuracion);
                }
                else
                {
                    // Si existe, actualizar los valores
                    configuracion.FrecuenciaRiego = configDto.FrecuenciaRiego;
                    configuracion.HorarioNotificacion = configDto.HorarioNotificacion;
                    configuracion.ActivarRiegoAutomatico = configDto.ActivarRiegoAutomatico;
                    configuracion.TipoAlertaSensor = configDto.TipoAlertaSensor;
                    configuracion.HabilitarRecomendacionesEstacionales = configDto.HabilitarRecomendacionesEstacionales;
                    _context.ConfiguracionNotificaciones.Update(configuracion);
                }

                await _context.SaveChangesAsync();

                // Devolver el DTO actualizado (que es lo que se recibió)
                return Ok(configDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
