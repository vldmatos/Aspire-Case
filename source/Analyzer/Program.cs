using MassTransit;

namespace Analyzer;

public class Program
{
    public static void Main(string[] args)
    {
        //wait for the RabbitMQ server to start
        Thread.Sleep(TimeSpan.FromSeconds(15));

        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults();

        var messaging = builder.Configuration.GetConnectionString("messaging");
        if (!string.IsNullOrWhiteSpace(messaging))
        {
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<SensorConsumer>();
                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, configuration) =>
                {
                    configuration.Host(new Uri(messaging), c => { });
                    configuration.ConfigureEndpoints(context);
                });
            });
        }
        
        builder.Services.AddMetrics();
        builder.Services.AddHttpClient();
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}