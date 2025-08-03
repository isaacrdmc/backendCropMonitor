using MQTTnet;
using System.Text;
//using MQTTnet.Client;
//using MQTTnet.Client.Options;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CropMonitor.Data; // Tu contexto EF



namespace CropMonitor.Services
{
    public class MqttService : BackgroundService
    {
        // 
        private readonly ILogger<MqttService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        // 
        public MqttService(ILogger<MqttService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        // 
        protected override async Task ExecuteAsync(System.Threading.CancellationToken stoppingToken)
        {
            // 
            var factory = new MqttClientConnectResultFactory();
            var mqttClient = factory.CreateMqttClient();

            // 
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com")
                .WithClientId("CropMonitorClient_" + Guid.NewGuid())
                .Build();

            // 
            mqttClient.UserConnectedHandler(async e =>
            {
                _logger.LogInformation("Connected to MQTT broker.");

                await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic("valor-temperatura")
                    .Build());

                // 
                _logger.LogInformation("Subscribed to topic 'valor-temperatura'.");
            });

            // 
            mqttClient.UseApplicationMessageReceivedHandler(equals =>
            {
                var payload = Encoding.UTF8.GetString(equals.ApplicationMessage.Payload);
                _logger.LogInformation($"Received message: {payload}");

                // Procesamos los datos y los guardamos dentro de la BD:
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<CropMonitorContext>();


            });

            // 
            await mqttClient.ConnectAsync(options, stoppingToken);

        }
    }
}
