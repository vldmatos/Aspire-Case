var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Sensor_Manager>("sensor-manager");

builder.AddProject<Projects.Data_Collector>("data-collector");

builder.AddProject<Projects.Calibrator_Analyzer>("calibrator-analyzer");

builder.Build().Run();
