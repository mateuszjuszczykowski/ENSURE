using DATA_processor;
using mqtt_client;

Log.Logger = LoggerConfig.GetLoggerConfiguration().CreateLogger();
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton(Log.Logger);
        services.AddHostedService<Processor>();
    })
    .Build();

host.Run();