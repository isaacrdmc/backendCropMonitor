using CropMonitor.Data;
using CropMonitor.DTOs.Modulos;
using CropMonitor.DTOs.Sensores;
using CropMonitor.Models.AppMovil;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; // Para obtener el ID del usuario del token
using System.Threading.Tasks;

namespace CropMonitorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Todos los endpoints en este controlador requieren autenticación
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
            // Obtener el UsuarioID del token JWT del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            // Obtener los módulos del usuario, incluyendo los sensores para contar los cultivos
            var modulos = await _context.Modulos
                                        .Where(m => m.UsuarioID == userId)
                                        // Incluye los sensores para poder calcular CantidadCultivosActual
                                        .Include(m => m.Sensores)
                                        .ToListAsync();

            var moduloListDtos = modulos.Select(m => new ModuloListDto
            {
                ModuloID = m.ModuloID,
                NombreModulo = m.NombreModulo,
                Estado = m.Estado,
                DiasEnFuncionamiento = m.DiasEnFuncionamiento ?? 0, // Manejo de nulos si la DB lo permite
                CantidadCultivosActual = m.Sensores.Count(s => s.CultivoID.HasValue),
                CantidadCultivosMax = m.CantidadCultivosMax ?? 0 // Conversión explícita para manejar valores nulos
            }).ToList();

            return Ok(moduloListDtos);
        }

        /// <summary>
        /// Crea un nuevo módulo para el usuario autenticado y automáticamente configura sus 4 espacios de medidores,
        /// cada uno con sensores de Temperatura, Humedad del Suelo y Luz.
        /// (Botón "Agregar nuevo módulo +" en Pantalla 1)
        /// </summary>
        /// <param name="moduloDto">Datos para la creación del módulo.</param>
        /// <returns>El ModuloListDto del módulo creado.</returns>
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
                CantidadCultivosActual = 0, // Siempre inicia con 0 cultivos
                CantidadCultivosMax = 4,   // *** Valor fijo de 4 espacios de cultivo ***
                UsuarioID = userId // Asigna el módulo al usuario autenticado
            };

            _context.Modulos.Add(nuevoModulo);
            await _context.SaveChangesAsync(); // Guarda el módulo primero para obtener su ModuloID

            // *** CREACIÓN AUTOMÁTICA DE SENSORES PARA CADA ESPACIO DEL MÓDULO ***
            // Define los tipos de sensores que cada espacio tendrá
            var sensorTypes = new List<(string Tipo, string Unidad)>
            {
                ("Temperatura", "°C"),
                ("Humedad", "%"),
                ("Luz", "Lux")
            };

            // Para cada uno de los 4 espacios de cultivo (CantidadCultivosMax)
            for (int i = 0; i < nuevoModulo.CantidadCultivosMax; i++) // 'i' será 0, 1, 2, 3
            {
                // Por cada espacio, crea los 3 tipos de sensores definidos
                foreach (var sensorType in sensorTypes)
                {
                    _context.Sensores.Add(new Sensor
                    {
                        ModuloID = nuevoModulo.ModuloID,
                        TipoSensor = sensorType.Tipo,
                        UnidadMedida = sensorType.Unidad,
                        UltimaLectura = null,
                        ValorLectura = null,
                        EstadoRiego = (sensorType.Tipo == "Humedad") ? "Inactivo" : "N/A",
                        EsAcuaHidroponico = false,
                        CultivoID = null, // Inicialmente sin cultivo asignado
                        MedidorSlotIndex = i // *** ASIGNA EL ÍNDICE DEL SLOT AQUÍ ***
                    });
                }
            }
            await _context.SaveChangesAsync(); // Guarda todos los sensores creados

            // Mapear a DTO para la respuesta
            var responseDto = new ModuloListDto
            {
                ModuloID = nuevoModulo.ModuloID,
                NombreModulo = nuevoModulo.NombreModulo,
                Estado = nuevoModulo.Estado,
                DiasEnFuncionamiento = nuevoModulo.DiasEnFuncionamiento ?? 0,
                CantidadCultivosActual = 0, // Recién creado, no tiene cultivos asignados a los espacios
                CantidadCultivosMax = nuevoModulo.CantidadCultivosMax ?? 0 // Se tomará el valor de 4
            };

            return CreatedAtAction(nameof(GetModulosUsuario), responseDto); // Retorna 201 Created
        }


        /// <summary>
        /// Obtiene la lista de medidores (sensores) dentro de un módulo específico.
        /// (Pantalla 2: Vista de cultivos dentro de un módulo)
        /// </summary>
        /// <param name="moduloId">ID del módulo.</param>
        /// <returns>Lista de SensorInModuloDto.</returns>
        [HttpGet("{moduloId}/sensores")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SensorInModuloDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSensoresByModulo(int moduloId)
        {
            // Validar que el módulo pertenece al usuario autenticado
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

            // Obtener los sensores de ese módulo, incluyendo la información del cultivo asociado si existe
            var sensores = await _context.Sensores
                                        .Where(s => s.ModuloID == moduloId)
                                        .Include(s => s.Cultivo) // Incluir información del cultivo
                                        .ToListAsync();

            var sensorInModuloDtos = sensores.Select(s => new SensorInModuloDto
            {
                SensorID = s.SensorID,
                TipoSensor = s.TipoSensor,
                UnidadMedida = s.UnidadMedida,
                ValorLectura = s.ValorLectura,
                EstadoRiego = s.EstadoRiego, //
                CultivoID = s.CultivoID,
                CultivoNombre = s.Cultivo?.Nombre, // Acceso seguro con ?.
                CultivoImagenURL = s.Cultivo?.ImagenURL != null ? $"/{s.Cultivo.ImagenURL.Replace("\\", "/")}" : null // Asegura URL relativa
            }).ToList();

            return Ok(sensorInModuloDtos);
        }

        /// <summary>
        /// Obtiene los detalles de un sensor específico.
        /// (Pantalla 3: Detalle de un medidor)
        /// </summary>
        /// <param name="sensorId">ID del sensor.</param>
        /// <returns>SensorDetailDto.</returns>
        [HttpGet("sensores/{sensorId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SensorDetailDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSensorDetail(int sensorId)
        {
            // Validar que el sensor pertenece a un módulo del usuario autenticado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("No se pudo identificar al usuario autenticado.");
            }

            var sensor = await _context.Sensores
                                        .Include(s => s.Modulo) // Incluir módulo para validar usuario
                                        .Include(s => s.Cultivo) // Incluir cultivo para sus detalles
                                            .ThenInclude(c => c.TipsCultivos) // Incluir tips del cultivo
                                        .Where(s => s.SensorID == sensorId && s.Modulo.UsuarioID == userId)
                                        .FirstOrDefaultAsync();

            if (sensor == null)
            {
                return NotFound($"Sensor con ID {sensorId} no encontrado o no pertenece a un módulo del usuario.");
            }

            // Mapear a DTO de detalle
            var sensorDetailDto = new SensorDetailDto
            {
                SensorID = sensor.SensorID,
                ModuloNombre = sensor.Modulo.NombreModulo, //
                TipoSensor = sensor.TipoSensor, //
                UnidadMedida = sensor.UnidadMedida, //
                ValorLectura = sensor.ValorLectura, //
                UltimaLectura = sensor.UltimaLectura, //
                EstadoRiego = sensor.EstadoRiego, //
                EsAcuaHidroponico = sensor.EsAcuaHidroponico, //

                // Detalles del cultivo asociado (si existe)
                CultivoID = sensor.CultivoID,
                CultivoNombre = sensor.Cultivo?.Nombre,
                CultivoImagenURL = sensor.Cultivo?.ImagenURL != null ? $"/{sensor.Cultivo.ImagenURL.Replace("\\", "/")}" : null,
                CultivoRequisitosClima = sensor.Cultivo?.RequisitosClima,
                CultivoRequisitosAgua = sensor.Cultivo?.RequisitosAgua,
                CultivoRequisitosLuz = sensor.Cultivo?.RequisitosLuz,

                // Tips para esta planta (si existen)
                TipsParaEstaPlanta = sensor.Cultivo?.TipsCultivos?.Select(t => t.DescripcionTip).ToList() ?? new List<string>() //
            };

            return Ok(sensorDetailDto);
        }

        /// <summary>
        /// Asigna un cultivo a un espacio de medidor específico dentro de un módulo.
        /// Todos los sensores dentro de ese MedidorSlotIndex serán asociados al cultivo.
        /// (Botón "Nuevo Cultivo +" en Pantalla 2)
        /// </summary>
        /// <param name="moduloId">ID del módulo.</param>
        /// <param name="medidorSlotIndex">Índice del espacio de medidor (0-3) al que se le asignará el cultivo.</param>
        /// <param name="cultivoId">ID del cultivo a asignar.</param>
        /// <returns>Mensaje de confirmación.</returns>
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

            // Verificar que el módulo existe y pertenece al usuario
            var modulo = await _context.Modulos
                                    .Where(m => m.ModuloID == moduloId && m.UsuarioID == userId)
                                    .FirstOrDefaultAsync();

            if (modulo == null)
            {
                return NotFound($"Módulo con ID {moduloId} no encontrado o no pertenece al usuario.");
            }

            // Validar que el medidorSlotIndex está dentro del rango esperado (0 a CantidadCultivosMax - 1)
            if (medidorSlotIndex < 0 || medidorSlotIndex >= modulo.CantidadCultivosMax)
            {
                return BadRequest($"El índice del medidor ({medidorSlotIndex}) está fuera del rango válido (0 a {modulo.CantidadCultivosMax - 1}).");
            }

            var cultivo = await _context.Cultivos.FindAsync(cultivoId);
            if (cultivo == null)
            {
                return BadRequest($"Cultivo con ID {cultivoId} no encontrado.");
            }

            // Obtener TODOS los sensores de este módulo que pertenecen a este MedidorSlotIndex
            var sensoresDelSlot = await _context.Sensores
                                                .Where(s => s.ModuloID == moduloId && s.MedidorSlotIndex == medidorSlotIndex)
                                                .ToListAsync();

            if (!sensoresDelSlot.Any())
            {
                return NotFound($"No se encontraron sensores para el ModuloID {moduloId} y MedidorSlotIndex {medidorSlotIndex}.");
            }

            // Asignar el CultivoID a CADA sensor en este slot
            foreach (var sensor in sensoresDelSlot)
            {
                sensor.CultivoID = cultivoId;
                _context.Entry(sensor).State = EntityState.Modified;
            }

            // Recalcular la CantidadCultivosActual en el Modulo
            // Ahora se cuenta cuántos slots de medidor tienen al menos un sensor con un CultivoID asignado
            modulo.CantidadCultivosActual = await _context.Sensores
                                                          .Where(s => s.ModuloID == moduloId && s.CultivoID.HasValue)
                                                          .Select(s => s.MedidorSlotIndex)
                                                          .Distinct()
                                                          .CountAsync();
            _context.Entry(modulo).State = EntityState.Modified;


            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Cultivo {cultivo.Nombre} asignado al medidor slot {medidorSlotIndex} del módulo {moduloId} exitosamente." });
        }

        // También necesitarás un endpoint para DESASIGNAR el cultivo de un slot completo.
        // Similar al anterior, pero asignando CultivoID = null a todos los sensores del slot.

        /// <summary>
        /// Desasigna un cultivo de un espacio de medidor específico dentro de un módulo.
        /// Todos los sensores dentro de ese MedidorSlotIndex dejarán de estar asociados a un cultivo.
        /// </summary>
        /// <param name="moduloId">ID del módulo.</param>
        /// <param name="medidorSlotIndex">Índice del espacio de medidor (0-3) del que se desasignará el cultivo.</param>
        /// <returns>Mensaje de confirmación.</returns>
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
            // Verificar si el slot ya está vacío
            if (!sensoresDelSlot.Any(s => s.CultivoID.HasValue))
            {
                return BadRequest($"El medidor slot {medidorSlotIndex} ya está vacío.");
            }


            foreach (var sensor in sensoresDelSlot)
            {
                sensor.CultivoID = null; // Desasigna el cultivo
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