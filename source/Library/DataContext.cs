using Library.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Library
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<Sensor> Sensors { get; set; } = default!;
    }

    public static class Extensions
    {
        public static void CreateDbIfNotExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<DataContext>();
            try
            {
                context.Database.EnsureCreated();
                DbInitializer.Initialize(context);
            }
            catch (Exception)
            {
            }
        }
    }

    public static class DbInitializer
    {
        private const int total = 10;
        public static void Initialize(DataContext context)
        {
            if (context.Sensors.Any())
                return;

            var sensors = new List<Sensor>(total);

            for (var i = 0; i < total; i++)
            {
                string name = Guid.NewGuid().ToString().ToUpper()[..6];
                int max = Random.Shared.Next(60, 80);
                int min = Random.Shared.Next(20, 40);

                sensors.Add(new Sensor
                {
                    Name = name,
                    Min = min,
                    Max = max,
                    Pressure = Random.Shared.Next(min, max)
                });
            }

            context.AddRange(sensors);

            context.SaveChanges();
        }
    }
}
