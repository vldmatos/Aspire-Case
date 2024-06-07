using Library;
using MassTransit;

namespace Manager;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.AddNpgsqlDbContext<DataContext>("sensorsDatabase");
        
        var messaging = builder.Configuration.GetConnectionString("messaging");
        if (!string.IsNullOrWhiteSpace(messaging))
        {
            builder.Services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, configuration) =>
                {
                    configuration.Host(new Uri(messaging), c => { });
                    configuration.ConfigureEndpoints(context);
                });
            });
        }

        builder.Services.AddMetrics();
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpClient();

        var application = builder.Build();
        
        application.MapDefaultEndpoints();
        application.UseSwagger();
        application.UseSwaggerUI();
        application.UseHttpsRedirection();
        application.UseAuthorization();

        application.MapEndpoint();

        application.Run();
    }
}
