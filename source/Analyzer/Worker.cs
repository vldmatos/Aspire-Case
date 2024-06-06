using Library.Business;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Analyzer;

public class Worker(ILogger<Worker> logger,
                    IHttpClientFactory httpClientFactory,
                    IConnection connection) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IHttpClientFactory _clientFactory = httpClientFactory;
    private readonly IConnection _connection = connection;

    private const string _queueName = "signals";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var message = await ReceiveidMessage();
        if (message == string.Empty)
            return;
    }

    private Sensor? Analize(string message)
    {
        var sensor = JsonSerializer.Deserialize<Sensor>(message);
        if (sensor is null)
            return sensor;

        if (!sensor.IsCalibrate)
        {
            var httpClient = _clientFactory.CreateClient();
            httpClient.BaseAddress = new Uri("https://manager");
            httpClient.PostAsync($"/maintenance/{sensor.Name}", null);

            _logger.LogWarning("Send to Maintenance to manager:{sensor}", message);
        }

        return sensor;
    }

    private Task<string> ReceiveidMessage()
    {
        IModel channel = _connection.CreateModel();
        channel.QueueDeclareNoWait(_queueName, true, false, false, null);
        string message = string.Empty;

        var eventingConsumer = new EventingBasicConsumer(channel);
        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        channel.BasicConsume(queue: _queueName, autoAck: false, consumer: eventingConsumer);

        eventingConsumer.Received += (model, content) =>
        {
            channel.BasicAck(deliveryTag: content.DeliveryTag, multiple: false);

            message = Encoding.UTF8.GetString(content.Body.ToArray());

            _logger.LogInformation("Received:{sensor}", message);

            var sensor = Analize(message);
        };
       
        return Task.FromResult(message);
    }
}
