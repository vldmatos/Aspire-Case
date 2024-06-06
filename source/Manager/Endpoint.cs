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
            endpoints.MapPost("/signal", (Sensor sensor, HttpContext httpContext, IConnection connection) =>
            {
                if (sensor is null)
                    return sensor;
                
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
         
            return endpoints;
        }
    }
}
