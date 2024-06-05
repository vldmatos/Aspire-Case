using Library.Business;
using RabbitMQ.Client;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Data.Collector;

public class Worker(ILogger<Worker> logger, 
                    IHttpClientFactory httpClientFactory, 
                    IConnection connection) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IHttpClientFactory _clientFactory = httpClientFactory;
    private readonly IConnection _connection = connection;

    private const string _queueName = "sensors";
   
    private List<Sensor>? _sensors;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var httpClient = _clientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://sensor-manager");

        IModel channel = _connection.CreateModel();
        channel.QueueDeclareNoWait(_queueName, true, false, false, null);
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        var response = await httpClient.GetAsync("/MapSensors", stoppingToken);
        if (response.IsSuccessStatusCode)
            _sensors = await response.Content.ReadFromJsonAsync<List<Sensor>>(stoppingToken);

        if (_sensors is not null)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var sensorsNotMaintenance = _sensors.Where(x => !x.Maintenance)
                                                    .ToList();

                foreach (var sensor in sensorsNotMaintenance)
                {
                    response = await httpClient.GetAsync($"/Status/{sensor.Name}", stoppingToken);
                    if (response.IsSuccessStatusCode)
                    {
                        var status = await response.Content.ReadFromJsonAsync<Sensor>(stoppingToken);
                        if (status is not null)
                        {
                            if (status.IsCalibrate)
                            {
                                _logger.LogInformation("OK: {isCalibrate} - Sensor: {name} - Pressure: {pressure} - Min: {min} | Max: {max}",
                                                     status.IsCalibrate, status.Name, status.Pressure, status.Calibration.Min, status.Calibration.Max);
                            }
                            else
                            {
                                _logger.LogWarning("Warning: Calibration Sensor: {name} - Pressure: {pressure} - Min: {min} | Max: {max}",
                                                    status.Name, status.Pressure, status.Calibration.Min, status.Calibration.Max);

                                channel.BasicPublish(exchange: string.Empty,
                                                     routingKey: _queueName,
                                                     basicProperties: properties,
                                                     body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(status)));

                                await httpClient.PostAsync($"/Maintenance/{status.Name}", null, stoppingToken);
                            }
                        }
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
