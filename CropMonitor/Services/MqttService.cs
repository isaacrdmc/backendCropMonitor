using MQTTnet;      // Libreria para trabajar con MQTT
using System.Text;      // Para decodificar los mensajes recibidos
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;      // Permite crear servicios en segundo plano
using Microsoft.Extensions.DependencyInjection;      // Para obtener instancias como el DbContext
using Microsoft.Extensions.Logging;
using CropMonitor.Data; // Donde se ubica el contexto de la BD
using CropMonitor.Models; // Entidades si lo necesitas
using System;


namespace CropMonitor.Services
{
    public class MqttService
    {
        // Creamos un contexto "DbContext" por cada mensaje recibido
        private readonly IServiceScopeFactory _scopeFactory;
        private IMqttClient mqttClient;

        // 
        public MqttService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // 
        public async Task StarAsync()
        {
            // 
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            // 
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com", 1883)
                .Build();

            // Nos suscribimos a los topics cuando el cliente se conecte
            _mqttClient.UseConnectedHandler(async e =>
            {
                // 
                Console.WriteLine("### COnectando con el broker ###");

                // Nos suscribimos a los topics de entradas:
                await _mqttClient.SubscribeAsync("valor-humedad-1");
                await _mqttClient.SubscribeAsync("valor-humedad-2");
                await _mqttClient.SubscribeAsync("valor-humedad-3");
                await _mqttClient.SubscribeAsync("valor-humedad-4");

                Console.WriteLine("## Subscrito a los topics ###");
            });

            
            // Ejecutamos la lógica cada vez que llega un mensaje MQTT
            _mqttClient.UseApplicationMessageReceivedHandler(equals =>
            {
                // 
                var topic = equals.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(equals.ApplicationMessage.Payload);

                // 
                Console.WriteLine($"### Mensaje recibido: {topic} - {payload}  ###");
            });

            // 
            await _mqttClient.ConnectAsync(options);
        }

        // Metodo para guardar la lectura enviada
        private async Task SaveReading(string topic, string payload)
        {
            // 
            using (var scope = _scopeFactory.CreateScope())
            {
                // 
                var db = scope.ServiceProvider.GetRequiredService<CropMonitorContext>();

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

                decimal valor = 0;
                if (!decimal.TryParse(payload, out valor))
                    return;

                // Guardamos la lectura
                var lectura = new LecturasSensores
                {
                    sensorID = sensorId,
                    TimesTamp = DateTime.Now,
                    valor = valor
                };

                // 
                db.LecturaSensores.Add(lectura);

                // Actualizar última lectura en tabla 'Sensores'
                var sensor = await db.Sensores.FindAsync(sensorId);
                if (sensor != null)
                {
                    // 
                    sensor.UltimaLectura = DateTime.Now;
                    sensor.ValorLectura = valor;
                }

                // 
                await db.SaveChangesAsync();
            }
        }
    }
}
