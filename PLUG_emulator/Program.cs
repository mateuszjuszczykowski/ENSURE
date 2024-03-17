using PLUG_emulator;
using PLUG_emulator.config;

Log.Logger = LoggerConfig.GetLoggerConfiguration().CreateLogger();
MqttClientConfig mqttClientConfig = new MqttClientConfig();
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton(Log.Logger);
        services.AddHostedService<Emulator>(provider => new Emulator(mqttClientConfig, "MQTT Client"));
    })
    .Build();

host.Run();