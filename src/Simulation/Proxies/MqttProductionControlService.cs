using System.Text;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace Simulation.Proxies;

public class MqttProductionControlService : IProductionControlService
{
    private IMqttClient _client;

    private MqttProductionControlService(IMqttClient mqttClient)
    {
        _client = mqttClient;
    }

    public static async Task<MqttProductionControlService> CreateAsync(int camNumber)
    {
        var mqttHost = Environment.GetEnvironmentVariable("MQTT_HOST") ?? "localhost";
        var factory = new MqttFactory();
        var client = factory.CreateMqttClient();
        var mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(mqttHost, 1883)
            .WithClientId($"camerasim{camNumber}")
            .Build();
        await client.ConnectAsync(mqttOptions, CancellationToken.None);
        return new MqttProductionControlService(client);
    }

    public async Task SendProductionEntryAsync(Production production)
    {
        var eventJson = JsonConvert.SerializeObject(production);
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("trafficcontrol/entrycam")
            .WithPayload(Encoding.UTF8.GetBytes(eventJson))
            .Build();
        await _client.PublishAsync(message, CancellationToken.None);
    }

    public async Task SendProductionExitAsync(Production production)
    {
        var eventJson = JsonConvert.SerializeObject(production);
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("trafficcontrol/exitcam")
            .WithPayload(Encoding.UTF8.GetBytes(eventJson))
            .Build();
        await _client.PublishAsync(message, CancellationToken.None);
    }
}
