namespace Data.Collector;

public class Program
{
    public static void Main(string[] args)
    {
        //wait for the RabbitMQ server to start
        Thread.Sleep(TimeSpan.FromSeconds(15));

        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults();
        builder.AddRabbitMQClient("messaging");

        builder.Services.AddHttpClient();
        builder.Services.AddHostedService<Worker>();

        var host = builder.Build();
        host.Run();
    }
}