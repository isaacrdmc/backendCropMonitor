


// Creamos el espacio para los endpoints del MQTT
using CropMonitor.Data;
using CropMonitor.Models.AppMovil;
using CropMonitor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CropMonitor.Controllers.Mqtt
{
    // 
    [ApiController]
    [Route("api/[controller]")]
    public class SensoresController : ControllerBase
    {

        // 
        private readonly CropMonitorDbContext _dbContext;
        private readonly MqttService _mqttService;

        // Creamos el contructor que recibe la instancia del DbContext y del servicio MQTT
        public SensoresController(CropMonitorDbContext dbContext, MqttService mqttService)
        {
            _dbContext = dbContext;
            _mqttService = mqttService;
        }



        // Endpoint para recibir lecturas del sensor:
        [HttpGet("lecturas")]
        public async Task<IActionResult> GetLecturas()
        {
            // 
            var lecturas = await _dbContext.Sensores
                .Select(s => new
                {
                    s.SensorID,
                    s.TipoSensor,
                    s.ValorLectura,
                    s.UnidadMedida
                })
                .ToListAsync();




            // Estructuramos los JSON de la salida:
            var sonico = lecturas
                .FirstOrDefault(s => s.TipoSensor == "Nivel agua");
            var humedad = lecturas
                .Where(s => s.TipoSensor == "Humedad del suelo")
                .ToList();
            var temperatura = lecturas
                .Where(s => s.TipoSensor == "Temperatura")
                .ToList();
            var luz = lecturas
                .Where(s => s.TipoSensor == "Luz")
                .ToList();
            var riego = lecturas
                .Where(s => s.TipoSensor == "Salida de riego")
                .ToList();


            var respuesta = new
            {
                Sonico = sonico,
                Humedad = humedad,
                Temperatura = temperatura,
                Luz = luz,
                Riego = riego
            };


            return Ok(respuesta);
        }

        // Endpoint para recibir notificaciones de sensores
        [HttpPost("encender-bomba")]
        public async Task<IActionResult> EncenderBomba([FromBody] BombaRequest request)
        {
            // 
            if (request == null || string.IsNullOrWhiteSpace(request.Topic))
            {
                // 
                return BadRequest("Datos inválidos.");
            }

            await _mqttService.PublicarComando(request.Topic, request.Mensaje);

            return Ok(new { status = "Comando enviado" });
        }




        // Clase interna para manejar la solicitud de encendido de bomba
        public class BombaRequest
        {
            public string Topic { get; set; } = string.Empty;
            public string Mensaje { get; set; } = string.Empty;
        }


    }
}
