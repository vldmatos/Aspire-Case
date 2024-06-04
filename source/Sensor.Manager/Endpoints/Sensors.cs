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

            endpoints.MapGet("/SensorStatus/{name}", (string name, HttpContext httpContext) =>
            {
                var sensors = httpContext.RequestServices.GetService<List<Library.Business.Sensor>>();
                return sensors?.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
            })
           .WithName("GetSensorStatus")
           .WithTags("Sensors")
           .WithOpenApi();

            endpoints.MapPost("/CalibrateSensor/{name}", (string name, HttpContext httpContext) =>
            {
                var sensors = httpContext.RequestServices.GetService<List<Library.Business.Sensor>>();
                var sensor = sensors?.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));

                sensor?.Calibrate();

                return sensor;
            })
            .WithName("CalibrateSensor")
            .WithTags("Sensors")
            .WithOpenApi();

            endpoints.MapPost("/DescalibrateSensor/{name}", (string name, HttpContext httpContext) =>
            {
                var sensors = httpContext.RequestServices.GetService<List<Library.Business.Sensor>>();
                var sensor = sensors?.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));
                
                if (sensor is null)
                    return sensor;

                sensor.Pressure += sensor.Calibration.Max;
                //sensor.Pressure -= sensor.Calibration.Min;

                return sensor;
            })
            .WithName("DescalibrateSensor")
            .WithTags("Sensors")
            .WithOpenApi();


            return endpoints;
        }
    }
}
