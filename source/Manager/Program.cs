using Library.Business;

namespace Manager;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();
        builder.AddRabbitMQClient("messaging");

        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton(Context.Sensors);

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
