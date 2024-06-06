var builder = DistributedApplication.CreateBuilder(args);

var manager = builder.AddProject<Projects.Manager>("manager");

var databases = builder.AddPostgres("databases")                       
                       .WithPgAdmin();

var sensorsDatabase = databases.AddDatabase("sensorsDatabase");

var messaging = builder.AddRabbitMQ("messaging")                      
                       .WithManagementPlugin();

var sensors = builder.AddProject<Projects.Sensors>("sensors")
                     .WithReference(sensorsDatabase)
                     .WithReference(manager);

builder.AddProject<Projects.Analyzer>("analyzer")
       .WithReference(manager)
       .WithReference(messaging);

manager.WithReference(messaging)
       .WithReference(sensors)
       .WithReference(sensorsDatabase);


builder.Build().Run();
