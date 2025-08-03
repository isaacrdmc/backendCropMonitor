using CropMonitor.Data; // Donde se ubica el contexto de la BD
using CropMonitor.Models.AppMovil;
using System.Text;      // Para decodificar los mensajes recibidos
using MQTTnet;      // Libreria para trabajar con MQTT
using CropMonitor.DTOs.Sensores;
using CropMonitor.Models; // Entidades si lo necesitas
using Microsoft.Extensions.DependencyInjection;      // Para obtener instancias como el DbContext
using Microsoft.Extensions.Hosting;      // Permite crear servicios en segundo plano
using Microsoft.Extensions.Logging;
using MQTTnet;              // MqttFactory
using MQTTnet.Client;       // IMqttClient
using MQTTnet.Client.Options; // MqttClientOptionsBuilder
using System;
using System.Threading.Tasks;


namespace CropMonitor.Services
{
    public class MqttService : IHostedService
    {
        // Creamos un contexto "DbContext" por cada mensaje recibido
        private readonly IServiceScopeFactory _scopeFactory;
        private IMqttClient _mqttClient;

        // 
        public MqttService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }




        // Metodo para suscribirme y leer todo lo que se publica en el broker
        // Tambien guardamos los datos en la BD.
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // 
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();


            // Nos suscribimos a los topics cuando el cliente se conecte
            _mqttClient.ConnectedAsync += async e =>
            {
                // 
                Console.WriteLine("### COnectando con el broker ###");

                // Nos suscribimos a los topics de entradas:
                await _mqttClient.SubscribeAsync("valor-humedad-1");
                await _mqttClient.SubscribeAsync("valor-humedad-2");
                await _mqttClient.SubscribeAsync("valor-humedad-3");
                await _mqttClient.SubscribeAsync("valor-humedad-4");

                Console.WriteLine("## Subscrito a los topics ###");
            };


            // Ejecutamos la lógica cada vez que llega un mensaje MQTT
            //_mqttClient.UseApplicationMessageReceivedHandler(equals =>
            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                // 
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                // 
                Console.WriteLine($"Mensaje recibido: {topic} - {payload}");
                await SaveReading(topic, payload);
            };

            // Nos conecctamos al broker
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com", 1883)
                .Build();


            // 
            await _mqttClient.ConnectAsync(options);
        }

        // Interfaz para que "IHostedService" funcione correctamente
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient != null)
            {
                await _mqttClient.DisconnectAsync();
            }
        }


        // Metodo para suscribirme y enviar datos para interactuar con la ESP32
        // Tambien guardamos los datos en la BD.
        private async Task SaveReading(string topic, string payload)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CropMonitorDbContext>();


            // 
            int sensorId = topic switch
            {
                "valor-humedad-1" => 1,
                "valor-humedad-2" => 2,
                "valor-humedad-3" => 3,
                "valor-humedad-4" => 4,
                _ => 0
            };

            // 
            if (sensorId == 0) return;

            if (!decimal.TryParse(payload, out decimal valor)) return;

            // Guardamos la lectura
            var lectura = new LecturaSensor
            {
                SensorID = sensorId,
                Timestamp = DateTime.Now,
                Valor = valor
            };


            // 
            db.LecturasSensores.Add(lectura);

            // Actualizar última lectura en tabla 'Sensores'
            var sensor = await db.Sensores.FindAsync(sensorId);
            if (sensor != null)
            {
                sensor.UltimaLectura = DateTime.Now;
                sensor.ValorLectura = valor;
            }

            await db.SaveChangesAsync();
        }
    }
}

