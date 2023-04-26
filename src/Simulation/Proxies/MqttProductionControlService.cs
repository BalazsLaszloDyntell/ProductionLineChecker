using System.Text;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace Simulation.Proxies;

public class MqttSignalProcessService : ISignalProcessService
{
    private IMqttClient _client;

    private MqttSignalProcessService(IMqttClient mqttClient)
    {
        _client = mqttClient;
    }

    public static async Task<MqttSignalProcessService> CreateAsync(int scanNumber)
    {
        var mqttHost = Environment.GetEnvironmentVariable("MQTT_HOST") ?? "localhost";
        var factory = new MqttFactory();
        var client = factory.CreateMqttClient();
        var mqttOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(mqttHost, 1883)
            .WithClientId($"scan{scanNumber}")
            .Build();
        await client.ConnectAsync(mqttOptions, CancellationToken.None);
        return new MqttSignalProcessService(client);
    }

    public async Task SendProductionEntryAsync(Production production)
    {
        var eventJson = JsonConvert.SerializeObject(production);
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("signalprocess/entrycheckpoint")
            .WithPayload(Encoding.UTF8.GetBytes(eventJson))
            .Build();
        await _client.PublishAsync(message, CancellationToken.None);
    }

    public async Task SendProductionExitAsync(Production production)
    {
        var eventJson = JsonConvert.SerializeObject(production);
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("signalprocess/exitcheckpoint")
            .WithPayload(Encoding.UTF8.GetBytes(eventJson))
            .Build();
        await _client.PublishAsync(message, CancellationToken.None);
    }
}
