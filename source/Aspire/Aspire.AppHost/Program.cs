var builder = DistributedApplication.CreateBuilder(args);

var rabbitMQ = builder.AddRabbitMQ("messaging")                      
                      .WithManagementPlugin();

var sensorManager = builder.AddProject<Projects.Sensor_Manager>("sensor-manager");

builder.AddProject<Projects.Data_Collector>("data-collector")
       .WithReference(sensorManager)
       .WithReference(rabbitMQ);

builder.AddProject<Projects.Calibrator_Analyzer>("calibrator-analyzer")
       .WithReference(sensorManager)
       .WithReference(rabbitMQ);


builder.Build().Run();
