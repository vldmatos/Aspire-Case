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

            endpoints.MapGet("/Status/{name}", (string name, HttpContext httpContext) =>
            {
                var sensors = httpContext.RequestServices.GetService<List<Library.Business.Sensor>>();
                return sensors?.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
            })
           .WithName("Status")
           .WithTags("Sensors")
           .WithOpenApi();

            endpoints.MapPost("/Calibrate/{name}", (string name, HttpContext httpContext) =>
            {
                var sensors = httpContext.RequestServices.GetService<List<Library.Business.Sensor>>();
                var sensor = sensors?.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));

                sensor?.Calibrate();

                return sensor;
            })
            .WithName("Calibrate")
            .WithTags("Sensors")
            .WithOpenApi();

            endpoints.MapPost("/Descalibrate/{name}", (string name, HttpContext httpContext) =>
            {
                var sensors = httpContext.RequestServices.GetService<List<Library.Business.Sensor>>();
                var sensor = sensors?.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
                
                if (sensor is null)
                    return sensor;

                sensor.Pressure += sensor.Calibration.Max;
                //sensor.Pressure -= sensor.Calibration.Min;

                return sensor;
            })
            .WithName("Descalibrate")
            .WithTags("Sensors")
            .WithOpenApi();

            endpoints.MapPost("/Maintenance/{name}", (string name, HttpContext httpContext) =>
            {
                var sensors = httpContext.RequestServices.GetService<List<Library.Business.Sensor>>();
                var sensor = sensors?.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));

                if (sensor is null)
                    return sensor;

                sensor.Maintenance = true;

                return sensor;
            })
            .WithName("Maintenance")
            .WithTags("Sensors")
            .WithOpenApi();


            return endpoints;
        }
    }
}
