using Library.Business;
using RabbitMQ.Client;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Data.Collector;

public class Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory, IConnection connection) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IHttpClientFactory _clientFactory = httpClientFactory;
    private readonly IConnection _connection = connection;
   
    private List<Sensor>? _sensors;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var httpClient = _clientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://sensor-manager");

        IModel channel = _connection.CreateModel();
        channel.QueueDeclareNoWait("sensors", true, false, false, null);
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;

        var response = await httpClient.GetAsync("/MapSensors", stoppingToken);
        if (response.IsSuccessStatusCode)
            _sensors = await response.Content.ReadFromJsonAsync<List<Sensor>>(stoppingToken);

        if (_sensors is not null)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var sensor in _sensors)
                {
                    response = await httpClient.GetAsync($"/SensorStatus/{sensor.Name}", stoppingToken);
                    if (response.IsSuccessStatusCode)
                    {
                        var status = await response.Content.ReadFromJsonAsync<Sensor>(stoppingToken);

                        if (status is not null)
                            _logger.LogInformation("Sensor: {name} - Pressure: {pressure} - Min: {min} | Max: {max} - OK: {isCalibrate}", 
                                                    status.Name, status.Pressure, status.Calibration.Min, status.Calibration.Max, status.IsCalibrate);

                        channel.BasicPublish(exchange: string.Empty, 
                                             routingKey: "sensors", 
                                             basicProperties: properties, 
                                             body: Encoding.UTF8.GetBytes(JsonSerializer.Serialize(sensor)));
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
