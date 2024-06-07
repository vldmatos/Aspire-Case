using Library.Business;
using MassTransit;

namespace Manager
{
    public static class Endpoint
    {
        private const string queue = "signals"; 

        public static IEndpointRouteBuilder MapEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/signal", async (Sensor sensor, HttpContext httpContext, IBus bus) =>
            {
                if (sensor is null)
                    return sensor;

                await bus.Publish(sensor);

                return sensor;
            })
            .WithName("Signal")
            .WithTags("Manager")
            .WithOpenApi();

            endpoints.MapPost("/maintenance/{name}", (string name, HttpContext httpContext, IHttpClientFactory httpClientFactory) =>
            {
                if (string.IsNullOrEmpty(name))
                    return;

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
