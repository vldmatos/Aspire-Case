var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddRabbitMQ("messaging")                      
                       .WithManagementPlugin();

var manager = builder.AddProject<Projects.Manager>("manager");

var sensors = builder.AddProject<Projects.Sensors>("sensors")
                     .WithReference(manager);

builder.AddProject<Projects.Analyzer>("analyzer")
       .WithReference(manager)
       .WithReference(messaging);

manager.WithReference(messaging)
       .WithReference(sensors);


builder.Build().Run();
