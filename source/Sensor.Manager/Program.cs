
using Library.Business;
using Sensor.Manager.Endpoints;

namespace Sensor.Manager;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        builder.Services.AddAuthorization();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton(Mapping.Sensors);

        var application = builder.Build();

        application.MapDefaultEndpoints();

        application.UseSwagger();
        application.UseSwaggerUI();        
        application.UseHttpsRedirection();
        application.UseAuthorization();

        application.MapSensors();

        application.Run();
    }
}
