using mqtt_client;
using MQTT_client.config;
using MqttClient = mqtt_client.MqttClientWorker;

Log.Logger = LoggerConfig.GetLoggerConfiguration().CreateLogger();
MqttClientConfig mqttClientConfig = new MqttClientConfig();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton(Log.Logger);
        services.AddHostedService<MqttClientWorker>(provider => new MqttClientWorker(mqttClientConfig, "MQTT Client"));
    })
    .Build();

host.Run();