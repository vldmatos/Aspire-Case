using Library;

namespace Manager;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.AddNpgsqlDbContext<DataContext>("sensorsDatabase");
        builder.AddRabbitMQClient("messaging");

        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpClient();

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
