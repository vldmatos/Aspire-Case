using Library;
using Library.Business;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Manager
{
    public static class Endpoint
    {
        private const string queue = "signals"; 

        public static IEndpointRouteBuilder MapEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/signal", (Sensor sensor, HttpContext httpContext, IConnection connection, DataContext dataContext) =>
            {
                if (sensor is null)
                    return sensor;

                var sensors = dataContext.Sensors.ToList();
                var received = sensors?.FirstOrDefault(item => string.Equals(item.Name, sensor.Name, StringComparison.OrdinalIgnoreCase));

                if (received is not null)
                    sensors?.Add(received);

                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queue, 
                                         durable: true, 
                                         exclusive: false, 
                                         autoDelete: false, 
                                         arguments: null);

                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(sensor));

                    channel.BasicPublish(exchange: string.Empty, 
                                         routingKey: queue, 
                                         basicProperties: null, 
                                         body: body);
                }

                return sensor;
            })
            .WithName("Signal")
            .WithTags("Manager")
            .WithOpenApi();

            endpoints.MapPost("/maintenance/{name}", (string name, HttpContext httpContext, IHttpClientFactory httpClientFactory, DataContext dataContext) =>
            {
                var sensors = dataContext.Sensors.ToList();
                var sensor = sensors?.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));

                if (sensor is null)
                    return sensor;

                var httpClient = httpClientFactory.CreateClient(); 
                httpClient.BaseAddress = new Uri("https://sensors");
                httpClient.PostAsync($"/calibrate/{sensor.Name}", null);

                return sensor;
            })
            .WithName("Maintenance")
            .WithTags("Sensors")
            .WithOpenApi();

            return endpoints;
        }
    }
}
