using mqtt_broker;
using mqtt_broker.config;

MqttServiceConfig mqttBrokerConfig = new();
Log.Logger = LoggerConfig.GetLoggerConfiguration().CreateLogger();

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    services.AddSingleton(Log.Logger);
    services.AddHostedService<MqttService>(provider => new MqttService(mqttBrokerConfig, "MQTT Broker"));
});

var host = builder.Build();

host.Run();
