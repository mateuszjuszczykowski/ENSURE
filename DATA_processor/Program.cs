using DATA_processor;
using DATA_processor.config;

Log.Logger = DATA_processor.config.LoggerConfig.GetLoggerConfiguration().CreateLogger();
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton(Log.Logger);
        services.AddHostedService<Processor>();
    })
    .Build();

host.Run();