using Library;

namespace Sensors;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.AddServiceDefaults();
        builder.AddNpgsqlDbContext<DataContext>("sensorsDatabase");

        builder.Services.AddMetrics();
        builder.Services.AddHttpClient();
        builder.Services.AddAuthorization();        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var application = builder.Build();

        application.CreateDbIfNotExists();        
        application.MapDefaultEndpoints();

        application.UseSwagger();
        application.UseSwaggerUI();
        
        application.UseHttpsRedirection();
        application.UseAuthorization();

        application.MapEndpoint();

        application.Run();
    }
}
