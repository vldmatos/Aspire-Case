using Library;
using Library.Business;
using MassTransit;
using System.Diagnostics.Metrics;

namespace Manager
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/signal", async 
                                (Sensor sensor, 
                                HttpContext httpContext, 
                                IBus bus, 
                                IMeterFactory meterFactory) =>
            {
                if (sensor is null)
                    return sensor;

                await bus.Publish(sensor);

                var meter = meterFactory.Create("Manager");
                var instrument = meter.CreateCounter<int>("Received-Signal");
                instrument.Add(1);

                return sensor;
            })
            .WithName("Signal")
            .WithTags("Manager")
            .WithOpenApi();

            endpoints.MapPost("/maintenance/{name}", 
                                (string name, 
                                HttpContext httpContext, 
                                IHttpClientFactory httpClientFactory, 
                                DataContext dataContext,
                                IMeterFactory meterFactory) =>
            {
                if (string.IsNullOrEmpty(name))
                    return;

                var sensor = dataContext.Sensors.FirstOrDefault(x => x.Name == name);
                if (sensor is null)
                    return;

                sensor.Maintenance = true;
                dataContext.SaveChanges();

                var meter = meterFactory.Create("Sensor-Maintenance");
                var instrument = meter.CreateCounter<int>(sensor.Name);
                instrument.Add(1);

                var httpClient = httpClientFactory.CreateClient(); 
                httpClient.BaseAddress = new Uri("https://sensors");
                httpClient.PostAsync($"/calibrate/{name}", null);

                return;
            })
            .WithName("Maintenance")
            .WithTags("Sensors")
            .WithOpenApi();

            return endpoints;
        }
    }
}
