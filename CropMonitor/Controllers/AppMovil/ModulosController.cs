using CropMonitor.Data;
using CropMonitor.DTOs.Modulos;
using CropMonitor.DTOs.Sensores;
using CropMonitor.Models.AppMovil;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CropMonitor.Controllers.AppMovil
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ModulosController : ControllerBase
    {
        private readonly CropMonitorDbContext _context;

        public ModulosController(CropMonitorDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene una lista de todos los módulos asociados al usuario autenticado.
        /// (Pantalla 1: Lista de Módulos)
        /// </summary>
        /// <returns>Lista de ModuloListDto.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ModuloListDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetModulosUsuario()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            var modulos = await _context.Modulos
                                        .Where(m => m.UsuarioID == userId)
                                        .Include(m => m.Sensores)
                                        .ToListAsync();

            var moduloListDtos = modulos.Select(m => new ModuloListDto
            {
                ModuloID = m.ModuloID,
                NombreModulo = m.NombreModulo,
                Estado = m.Estado,
                DiasEnFuncionamiento = m.DiasEnFuncionamiento ?? 0,
                // *** CAMBIO: Ahora contamos los MedidorSlotIndex distintos con un CultivoID ***
                CantidadCultivosActual = m.Sensores
                                          .Where(s => s.CultivoID.HasValue)
                                          .Select(s => s.MedidorSlotIndex)
                                          .Distinct() // Obtiene solo lo s índices únicos
                                          .Count(),  // Cuenta cuántos índices únicos hay
                CantidadCultivosMax = m.CantidadCultivosMax ?? 0
            }).ToList();

            return Ok(moduloListDtos);
        }

        /// <summary>
        /// Crea un nuevo módulo para el usuario autenticado y automáticamente configura sus 4 espacios de medidores.
        /// (Botón "Agregar nuevo módulo +" en Pantalla 1)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ModuloListDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateModulo([FromBody] ModuloCreateDto moduloDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            var nuevoModulo = new Modulo
            {
                NombreModulo = moduloDto.NombreModulo,
                Estado = moduloDto.Estado,
                DiasEnFuncionamiento = moduloDto.DiasEnFuncionamiento,
                CantidadCultivosActual = 0,
                CantidadCultivosMax = 4,
                UsuarioID = userId
            };

            _context.Modulos.Add(nuevoModulo);
            await _context.SaveChangesAsync();

            var sensorTypes = new List<(string Tipo, string Unidad)>
            {
                ("Temperatura", "°C"),
                ("Humedad", "%"),
                ("Luz", "Lux")
            };

            for (int i = 0; i < nuevoModulo.CantidadCultivosMax; i++)
            {
                foreach (var sensorType in sensorTypes)
                {
                    _context.Sensores.Add(new Sensor
                    {
                        ModuloID = nuevoModulo.ModuloID,
                        TipoSensor = sensorType.Tipo,
                        UnidadMedida = sensorType.Unidad,
                        UltimaLectura = null,
                        ValorLectura = null,
                        EstadoRiego = sensorType.Tipo == "Humedad" ? "Inactivo" : "N/A",
                        EsAcuaHidroponico = false,
                        CultivoID = null,
                        MedidorSlotIndex = i
                    });
                }
            }
            await _context.SaveChangesAsync();

            var responseDto = new ModuloListDto
            {
                ModuloID = nuevoModulo.ModuloID,
                NombreModulo = nuevoModulo.NombreModulo,
                Estado = nuevoModulo.Estado,
                DiasEnFuncionamiento = nuevoModulo.DiasEnFuncionamiento ?? 0,
                CantidadCultivosActual = 0,
                CantidadCultivosMax = nuevoModulo.CantidadCultivosMax ?? 0
            };

            return CreatedAtAction(nameof(GetModulosUsuario), responseDto);
        }


        /// <summary>
        /// Obtiene la lista de medidores (sensores) dentro de un módulo específico, agrupados por slot.
        /// (Pantalla 2: Vista de cultivos dentro de un módulo)
        /// </summary>
        /// <param name="moduloId">ID del módulo.</param>
        /// <returns>Lista de MedidorSlotDto.</returns>
        [HttpGet("{moduloId}/sensores")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MedidorSlotDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSensoresByModulo(int moduloId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            // Validar que el módulo existe y pertenece al usuario.
            var modulo = await _context.Modulos
                                     .Where(m => m.ModuloID == moduloId && m.UsuarioID == userId)
                                     .Include(m => m.Sensores)
                                     .ThenInclude(s => s.Cultivo)
                                     .FirstOrDefaultAsync();

            if (modulo == null)
            {
                return NotFound($"Módulo con ID {moduloId} no encontrado o no pertenece al usuario.");
            }

            // Mapear y agrupar los sensores por MedidorSlotIndex
            var slots = new List<MedidorSlotDto>();
            for (int i = 0; i < modulo.CantidadCultivosMax; i++)
            {
                var sensoresDelSlot = modulo.Sensores.Where(s => s.MedidorSlotIndex == i).ToList();

                // Si el slot tiene sensores, mapearlos al DTO
                if (sensoresDelSlot.Any())
                {
                    var primerSensor = sensoresDelSlot.First();
                    var slotDto = new MedidorSlotDto
                    {
                        MedidorSlotIndex = i,
                        CultivoID = primerSensor.CultivoID,
                        CultivoNombre = primerSensor.Cultivo?.Nombre,
                        CultivoImagenURL = primerSensor.Cultivo?.ImagenURL != null ? $"/{primerSensor.Cultivo.ImagenURL.Replace("\\", "/")}" : null,
                        EnUso = primerSensor.CultivoID.HasValue,
                        Sensores = sensoresDelSlot.Select(s => new SensorLecturaDto
                        {
                            SensorID = s.SensorID,
                            TipoSensor = s.TipoSensor,
                            UnidadMedida = s.UnidadMedida,
                            ValorLectura = s.ValorLectura,
                            EstadoRiego = s.EstadoRiego
                        }).ToList()
                    };
                    slots.Add(slotDto);
                }
                else
                {
                    // Si el slot no tiene sensores (caso improbable con tu lógica de creación), se agrega un slot vacío
                    slots.Add(new MedidorSlotDto
                    {
                        MedidorSlotIndex = i,
                        EnUso = false,
                        Sensores = new List<SensorLecturaDto>()
                    });
                }
            }

            return Ok(slots);
        }

        /// <summary>
        /// Obtiene los detalles de un sensor específico.
        /// (Pantalla 3: Detalle de un medidor)
        /// </summary>
        [HttpGet("sensores/{sensorId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SensorDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSensorDetail(int sensorId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            var sensor = await _context.Sensores
                                       .Include(s => s.Modulo)
                                       .Include(s => s.Cultivo)
                                            .ThenInclude(c => c.TipsCultivos)
                                       .Where(s => s.SensorID == sensorId && s.Modulo.UsuarioID == userId)
                                       .FirstOrDefaultAsync();

            if (sensor == null)
            {
                return NotFound($"Sensor con ID {sensorId} no encontrado o no pertenece a un módulo del usuario.");
            }

            var sensorDetailDto = new SensorDetailDto
            {
                SensorID = sensor.SensorID,
                ModuloNombre = sensor.Modulo.NombreModulo,
                TipoSensor = sensor.TipoSensor,
                UnidadMedida = sensor.UnidadMedida,
                ValorLectura = sensor.ValorLectura,
                UltimaLectura = sensor.UltimaLectura,
                EstadoRiego = sensor.EstadoRiego,
                EsAcuaHidroponico = sensor.EsAcuaHidroponico,

                CultivoID = sensor.CultivoID,
                CultivoNombre = sensor.Cultivo?.Nombre,
                CultivoImagenURL = sensor.Cultivo?.ImagenURL != null ? $"/{sensor.Cultivo.ImagenURL.Replace("\\", "/")}" : null,
                CultivoRequisitosClima = sensor.Cultivo?.RequisitosClima,
                CultivoRequisitosAgua = sensor.Cultivo?.RequisitosAgua,
                CultivoRequisitosLuz = sensor.Cultivo?.RequisitosLuz,

                TipsParaEstaPlanta = sensor.Cultivo?.TipsCultivos?.Select(t => t.DescripcionTip).ToList() ?? new List<string>()
            };

            return Ok(sensorDetailDto);
        }


        /// <summary>
        /// Asigna un cultivo a un espacio de medidor específico dentro de un módulo.
        /// </summary>
        [HttpPut("{moduloId}/medidores/{medidorSlotIndex}/asignar-cultivo/{cultivoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AsignarCultivoAMedidorSlot(int moduloId, int medidorSlotIndex, int cultivoId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            var modulo = await _context.Modulos
                                     .Where(m => m.ModuloID == moduloId && m.UsuarioID == userId)
                                     .FirstOrDefaultAsync();

            if (modulo == null)
            {
                return NotFound($"Módulo con ID {moduloId} no encontrado o no pertenece al usuario.");
            }

            if (medidorSlotIndex < 0 || medidorSlotIndex >= modulo.CantidadCultivosMax)
            {
                return BadRequest($"El índice del medidor ({medidorSlotIndex}) está fuera del rango válido (0 a {modulo.CantidadCultivosMax - 1}).");
            }

            var cultivo = await _context.Cultivos.FindAsync(cultivoId);
            if (cultivo == null)
            {
                return BadRequest($"Cultivo con ID {cultivoId} no encontrado.");
            }

            var sensoresDelSlot = await _context.Sensores
                                                 .Where(s => s.ModuloID == moduloId && s.MedidorSlotIndex == medidorSlotIndex)
                                                 .ToListAsync();

            if (!sensoresDelSlot.Any())
            {
                return NotFound($"No se encontraron sensores para el ModuloID {moduloId} y MedidorSlotIndex {medidorSlotIndex}.");
            }

            foreach (var sensor in sensoresDelSlot)
            {
                sensor.CultivoID = cultivoId;
                _context.Entry(sensor).State = EntityState.Modified;
            }

            modulo.CantidadCultivosActual = await _context.Sensores
                                                           .Where(s => s.ModuloID == moduloId && s.CultivoID.HasValue)
                                                           .Select(s => s.MedidorSlotIndex)
                                                           .Distinct()
                                                           .CountAsync();
            _context.Entry(modulo).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Cultivo {cultivo.Nombre} asignado al medidor slot {medidorSlotIndex} del módulo {moduloId} exitosamente." });
        }


        /// <summary>
        /// Desasigna un cultivo de un espacio de medidor específico dentro de un módulo.
        /// </summary>
        [HttpPut("{moduloId}/medidores/{medidorSlotIndex}/desasignar-cultivo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DesasignarCultivoDeMedidorSlot(int moduloId, int medidorSlotIndex)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            var modulo = await _context.Modulos
                                     .Where(m => m.ModuloID == moduloId && m.UsuarioID == userId)
                                     .FirstOrDefaultAsync();

            if (modulo == null)
            {
                return NotFound($"Módulo con ID {moduloId} no encontrado o no pertenece al usuario.");
            }

            if (medidorSlotIndex < 0 || medidorSlotIndex >= modulo.CantidadCultivosMax)
            {
                return BadRequest($"El índice del medidor ({medidorSlotIndex}) está fuera del rango válido (0 a {modulo.CantidadCultivosMax - 1}).");
            }

            var sensoresDelSlot = await _context.Sensores
                                                 .Where(s => s.ModuloID == moduloId && s.MedidorSlotIndex == medidorSlotIndex)
                                                 .ToListAsync();

            if (!sensoresDelSlot.Any())
            {
                return NotFound($"No se encontraron sensores para el ModuloID {moduloId} y MedidorSlotIndex {medidorSlotIndex}.");
            }

            if (!sensoresDelSlot.Any(s => s.CultivoID.HasValue))
            {
                return BadRequest($"El medidor slot {medidorSlotIndex} ya está vacío.");
            }

            foreach (var sensor in sensoresDelSlot)
            {
                sensor.CultivoID = null;
                _context.Entry(sensor).State = EntityState.Modified;
            }

            modulo.CantidadCultivosActual = await _context.Sensores
                                                           .Where(s => s.ModuloID == moduloId && s.CultivoID.HasValue)
                                                           .Select(s => s.MedidorSlotIndex)
                                                           .Distinct()
                                                           .CountAsync();
            _context.Entry(modulo).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Cultivo desasignado del medidor slot {medidorSlotIndex} del módulo {moduloId} exitosamente." });
        }
    }
}