using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Calibrator.Analyzer;

public class Worker(ILogger<Worker> logger,
                    IHttpClientFactory httpClientFactory,
                    IConnection connection) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IHttpClientFactory _clientFactory = httpClientFactory;
    private readonly IConnection _connection = connection;

    private const string _queueName = "sensors";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var httpClient = _clientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://sensor-manager");

        IModel channel = _connection.CreateModel();
        channel.QueueDeclareNoWait(_queueName, true, false, false, null);
        string sensor = string.Empty;

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        var eventingConsumer = new EventingBasicConsumer(channel);

        eventingConsumer.Received += (model, content) =>
        {
            channel.BasicAck(deliveryTag: content.DeliveryTag, multiple: false);

            sensor = Encoding.UTF8.GetString(content.Body.ToArray());

            _logger.LogInformation("Message:{sensor}", sensor);
        };

        channel.BasicConsume(queue: _queueName, autoAck: false, consumer: eventingConsumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            sensor = string.Empty;
        }
    }
}
