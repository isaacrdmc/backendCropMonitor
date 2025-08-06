using MQTTnet;      // Libreria para trabajar con MQTT
using CropMonitor.Data; // Donde se ubica el contexto de la BD
using CropMonitor.Models.AppMovil;
using System.Text;      // Para decodificar los mensajes recibidos
using CropMonitor.DTOs.Sensores;
using CropMonitor.Models; // Entidades si lo necesitas
using Microsoft.Extensions.DependencyInjection;      // Para obtener instancias como el DbContext
using Microsoft.Extensions.Hosting;      // Permite crear servicios en segundo plano
using Microsoft.Extensions.Logging;
//using MQTTnet.Client;       // IMqttClient
//using MQTTnet.Client.Options; // MqttClientOptionsBuilder
using System;
using System.Threading.Tasks;



namespace CropMonitor.Services
{
    public class MqttService : IHostedService
    {
        // Creamos un contexto "DbContext" por cada mensaje recibido
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MqttService> _logger;
        private IMqttClient _mqttClient;
        public MqttService(IServiceScopeFactory scopeFactory, ILogger<MqttService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        // Metodo para suscribirme y leer todo lo que se publica en el broker
        // Tambien guardamos los datos en la BD.
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                // OPCIÓN 1: Con MqttFactory (compatible con v5)
                //var factory = new MqttFactory();
                //_mqttClient = factory.CreateMqttClient();

                // OPCIÓN 2: Con MqttClientFactory (nuevo en v5) - Más directo
                var clientFactory = new MqttClientFactory();
                _mqttClient = clientFactory.CreateMqttClient();

                // OPCIÓN 3: Directo (más simple para v5)
                // _mqttClient = new MqttClientFactory().CreateMqttClient();




                // Nos suscribimos a los topics cuando el cliente se conecte
                _mqttClient.ConnectedAsync += async e =>
                {
                    try
                    {
                        // 
                        Console.WriteLine("### COnectando con el broker ###");

                        // Nos suscribimos a los topics de entradas:
                        //await _mqttClient.SubscribeAsync("valor-humedad-1");
                        //await _mqttClient.SubscribeAsync("valor-humedad-2");
                        //await _mqttClient.SubscribeAsync("valor-humedad-3");
                        //await _mqttClient.SubscribeAsync("valor-humedad-4");

                        //await _mqttClient.SubscribeAsync("valor-temperatura-1");
                        //await _mqttClient.SubscribeAsync("valor-temperatura-2");
                        //await _mqttClient.SubscribeAsync("valor-temperatura-3");
                        //await _mqttClient.SubscribeAsync("valor-temperatura-4");




                        // Nos suscribimos a los topics de entradas:

                        // Usando el comodin "#" le indicamos que se suscriba a TODOS los topics del broker conectado
                        await _mqttClient.SubscribeAsync("#");



                        //string[] topics = {
                        //    "valor-humedad-1", "valor-humedad-2", "valor-humedad-3", "valor-humedad-4",
                        //    "valor-temperatura-1", "valor-temperatura-2", "valor-temperatura-3", "valor-temperatura-4"
                        //};


                        //// Nos suscribimos a todos los topics de una vez
                        //foreach (var topic in topics)
                        //{
                        //    // Nos suscribimos a cada topic
                        //    await _mqttClient.SubscribeAsync(topic);

                        //}

                        Console.WriteLine("## Subscrito a los topics ###");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al suscribirse a los topics.");
                    }
                };



                // Ejecutamos la lógica cada vez que llega un mensaje MQTT
                //_mqttClient.UseApplicationMessageReceivedHandler(equals =>
                _mqttClient.ApplicationMessageReceivedAsync += async e =>
                {
                    try
                    {
                        // 
                        var topic = e.ApplicationMessage.Topic;
                        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                        // 
                        Console.WriteLine($"Mensaje recibido: {topic} - {payload}");
                        await SaveReading(topic, payload);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error procesando el mensaje: {ex.Message}");
                    }
                };


                // Manejamso la reconexió en caso de que se pierda la conexión:
                _mqttClient.DisconnectedAsync += async e =>
                {
                    _logger.LogWarning("Desconectado del broker. Intentando reocnexión...");

                    // 
                    int retryDelaySeconds = 5000; // 5 segundos
                    int maxRetryDelaySeconds = 60000; // 1 minuto

                    // 
                    while (!_mqttClient.IsConnected)
                    {
                        try
                        {
                            // 
                            await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
                            await _mqttClient.ReconnectAsync();
                            Console.WriteLine("Reconexión exitosa.");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error durante reconexión. Reintentando...");
                            retryDelaySeconds = Math.Min(retryDelaySeconds * 2, maxRetryDelaySeconds);
                        }
                    }
                };

                // Nos conecctamos al broker
                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer("c89a75e945ca4a3590a418760638ab79.s1.eu.hivemq.cloud", 8883)
                    .WithCredentials("hivemq.webclient.1753150694481", "Q78*&UOb3<McAwqxt$4V")
                    .WithTlsOptions(options =>
                    {
                        options.WithCertificateValidationHandler(context => true);
                    })
                    .Build();


                // 
                await _mqttClient.ConnectAsync(options);
            
            }catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al iniciar el servicio MQTT: {ex.Message}");

            }
        }




        // Interfaz para que "IHostedService" funcione correctamente
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_mqttClient != null)
                {
                    await _mqttClient.DisconnectAsync();
                    Console.WriteLine("### Desconectado del broker ###");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al detener el servicio MQTT: {ex.Message}");
            }
        }




        // Metodo para suscribirme y enviar datos para interactuar con la ESP32
        // Tambien guardamos los datos en la BD.
        private async Task SaveReading(string topic, string payload)
        {
            // Creamos un bucle para manejar la conexión a la base de datos:
            
            const int maxAttempts = 3;

            for (int attemp = 1; attemp < maxAttempts; attemp++)
            {
                try
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

                        "valor-temperatura-1" => 5,
                        "valor-temperatura-2" => 6,
                        "valor-temperatura-3" => 7,
                        "valor-temperatura-4" => 8,

                        "valor-luz-1" => 9,
                        "valor-luz-2" => 10,
                        "valor-luz-3" => 11,
                        "valor-luz-4" => 12,

                        "salida-riego-1" => 13,
                        "salida-riego-2" => 14,
                        "salida-riego-3" => 15,
                        "salida-riego-4" => 16,

                        "valor-sonico" => 17,

                        _ => 0      // Ignoramos los valores que no necesitmaos guardar
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


                    // Ahora vamos a guardar la lectura en la base de datos:

                    db.LecturasSensores.Add(lectura);

                    // Actualizar última lectura en tabla 'Sensores'
                    var sensor = await db.Sensores.FindAsync(sensorId);
                    if (sensor != null)
                    {
                        sensor.UltimaLectura = DateTime.Now;
                        sensor.ValorLectura = valor;
                    }

                    await db.SaveChangesAsync();
                    Console.WriteLine($"### Lectura guardada para sensor {sensorId} ###");

                    // Salimos del bucle si todo ha ido bien:
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error guardando la lectura. Intento {attemp} de {maxAttempts}.");

                    // 
                    if (attemp == maxAttempts)
                    {
                        _logger.LogError($"Fallo persistente al guardar la lectura después de {maxAttempts} intentos.");
                    } else
                    {
                        // Esperamos un poco antes de reintentar
                        await Task.Delay(TimeSpan.FromSeconds(2 * attemp));
                    }
                }

            }


        }
    
    
        // Metodo para publicar comandos a la ESP32:
        public async Task PublicarComando(string topic, string mensaje)
        {
            try
            {
                // 
                if (_mqttClient?.IsConnected ?? false)
                {
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(mensaje)
                        .Build();

                    // 
                    await _mqttClient.PublishAsync(message);
                    Console.WriteLine($"## Comando publicado: {topic} - {mensaje} ###");
                } else
                {
                    Console.WriteLine("## No conectado al broker, no se puede publicar ##");
                }
            } catch(Exception ex)
            {
                Console.WriteLine($"## No conectado al broker, no se puede publicar ###");
            }
        }
    
    }
}

