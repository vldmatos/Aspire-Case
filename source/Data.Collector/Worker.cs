using Library.Business;
using System.Net.Http.Json;

namespace Data.Collector;

public class Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IHttpClientFactory _clientFactory = httpClientFactory;

    private List<Sensor>? _sensors;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var httpClient = _clientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("https://sensor-manager"); 

        var response = await httpClient.GetAsync("/MapSensors", stoppingToken);
        if (response.IsSuccessStatusCode)
            _sensors = await response.Content.ReadFromJsonAsync<List<Sensor>>(stoppingToken);

        if (_sensors is not null)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var sensor in _sensors)
                {
                    _logger.LogInformation("Sensor: {name}", sensor.Name);
                    response = await httpClient.GetAsync($"/SensorStatus/{sensor.Name}", stoppingToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var status = await response.Content.ReadFromJsonAsync<Sensor>(stoppingToken);

                        if (status is not null)
                            _logger.LogInformation("Sensor: {name} - Pressure: {pressure}", status.Name, status.Pressure);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }
    }
}
