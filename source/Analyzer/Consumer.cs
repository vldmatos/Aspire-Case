using Library.Business;
using MassTransit;
using System.Diagnostics.Metrics;
using System.Text.Json;

namespace Analyzer
{
    public class SensorConsumer(ILogger<SensorConsumer> logger, 
                                IHttpClientFactory httpClientFactory,
                                IMeterFactory meterFactory) : IConsumer<Sensor>
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
                var meter = meterFactory.Create("Analyzer");
                var instrument = meter.CreateCounter<int>("Uncalibrated-Sensor");
                instrument.Add(1);

                var httpClient = httpClientFactory.CreateClient();
                httpClient.BaseAddress = new Uri("https://manager");
                httpClient.PostAsync($"/maintenance/{sensor.Name}", null);

                logger.LogInformation("Send to Maintenance to manager:{sensor}", JsonSerializer.Serialize(sensor));
            }

            return sensor;
        }
    }
}
