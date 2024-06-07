using Library;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Text;
using System.Text.Json;

namespace Sensors
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/sensors", 
                                (HttpContext httpContext, 
                                DataContext dataContext) =>
            {
                return dataContext.Sensors.ToList();
            })
            .WithName("Sensors")
            .WithTags("Sensors")
            .WithOpenApi();

            endpoints.MapPost("/calibrate/{name}", 
                                (string name, 
                                HttpContext httpContext, 
                                DataContext dataContext, 
                                IMeterFactory meterFactory) =>
            {
                var sensors = dataContext.Sensors.ToList();
                var sensor = sensors?.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));

                if (sensor is null)
                    return sensor;

                sensor.Maintenance = true;
                sensor.Calibrate();

                dataContext.SaveChanges();

                var meter = meterFactory.Create("Sensor");
                var instrument = meter.CreateCounter<int>("Sensor-Calibrate");
                instrument.Add(1);

                return sensor;
            })
            .WithName("Calibrate")
            .WithTags("Sensors")
            .WithOpenApi();

            endpoints.MapPost("/send-signal/{count}", 
                                (int count, 
                                HttpContext httpContext, 
                                IHttpClientFactory httpClientFactory, 
                                DataContext dataContext, 
                                IMeterFactory meterFactory) => 
           {
               var sensors = dataContext.Sensors.AsNoTracking()
                                                .ToList();
               if (sensors is null)
                    return;

               var client = httpClientFactory.CreateClient();
               client.BaseAddress = new Uri("https://manager");

               for(int i = 0; i < count; i++)  
               {
                    Parallel.ForEach(sensors, sensor =>
                    {
                        var content = new StringContent(JsonSerializer.Serialize(sensor), Encoding.UTF8, "application/json");
                        client.PostAsync("/signal", content);

                        var meter = meterFactory.Create("Sensor");
                        var instrument = meter.CreateCounter<int>("Send-Signal");
                        instrument.Add(1);
                    }); 
               }

               return;
           })
           .WithName("Send-Signal")
           .WithTags("Sensors")
           .WithOpenApi();

            endpoints.MapPost("/decalibrate/{name}", 
                                (string name, 
                                HttpContext httpContext, 
                                DataContext dataContext,
                                IMeterFactory meterFactory) =>
            {
                var sensors = dataContext.Sensors.ToList();
                var sensor = sensors?.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));

                if (sensor is null)
                    return sensor;

                sensor.Pressure = sensor.Min - 1;

                dataContext.SaveChanges();

                return sensor;
            })
            .WithName("Decalibrate ")
            .WithTags("Sensors")
            .WithOpenApi();

            return endpoints;
        }
    }
}
