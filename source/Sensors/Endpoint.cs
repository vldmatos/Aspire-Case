using Library.Business;
using System.Text;
using System.Text.Json;

namespace Sensors
{
    public static class Endpoint
    {
        public static IEndpointRouteBuilder MapEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/calibrate/{name}", (string name, HttpContext httpContext) =>
            {
                var sensors = httpContext.RequestServices.GetService<List<Sensor>>();
                var sensor = sensors?.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));

                if (sensor is null)
                    return sensor;

                sensor.Maintenance = true;
                sensor.Calibrate();

                return sensor;
            })
            .WithName("Calibrate")
            .WithTags("Sensors")
            .WithOpenApi();

            endpoints.MapPost("/send-signal/{count}", (int count, HttpContext httpContext, IHttpClientFactory httpClientFactory) => 
           {
               var sensors = httpContext.RequestServices.GetService<List<Sensor>>();
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
                    }); 
               }

               return;
           })
           .WithName("Send-Signal")
           .WithTags("Sensors")
           .WithOpenApi();

            return endpoints;
        }
    }
}
