namespace Sensor.Manager.Endpoints
{
    public static class Sensors
    {
        public static IEndpointRouteBuilder MapSensors(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/MapSensors", (HttpContext httpContext) => 
            {
                return httpContext.RequestServices.GetService<List<Library.Business.Sensor>>();
            })
            .WithName("GetSensors")
            .WithTags("Sensors")
            .WithOpenApi();
            
            return endpoints;
        }
    }
}
