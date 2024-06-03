var builder = DistributedApplication.CreateBuilder(args);

var rabbitMQ = builder.AddRabbitMQ("rabbitMQ")
                      .WithEnvironment("RABBITMQ_DEFAULT_USER", "admin")
                      .WithEnvironment("RABBITMQ_DEFAULT_PASS", "admin")
                      .WithManagementPlugin();

builder.AddProject<Projects.Sensor_Manager>("sensor-manager");

builder.AddProject<Projects.Data_Collector>("data-collector")           
       .WithReference(rabbitMQ);

builder.AddProject<Projects.Calibrator_Analyzer>("calibrator-analyzer")
       .WithReference(rabbitMQ);

builder.Build().Run();
