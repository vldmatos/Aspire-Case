using Library.Business;
using MassTransit;
using System.Text.Json;

namespace Analyzer
{
    public class SensorConsumer(ILogger<SensorConsumer> logger, IHttpClientFactory httpClientFactory) : IConsumer<Sensor>
    {
        public Task Consume(ConsumeContext<Sensor> context)
        {
            logger.LogInformation("Received:{sensor}", JsonSerializer.Serialize(context.Message));
            
            Analize(context.Message);
            
            return Task.CompletedTask;
        }

        private Sensor? Analize(Sensor sensor)
        {
            if (!sensor.IsCalibrate)
            {
                var httpClient = httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("https://manager");
                httpClient.PostAsync($"/maintenance/{sensor.Name}", null);

                logger.LogInformation("Send to Maintenance to manager:{sensor}", JsonSerializer.Serialize(sensor));
            }

            return sensor;
        }
    }
}
