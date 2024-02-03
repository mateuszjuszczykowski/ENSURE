using mqtt_broker;
using mqtt_broker.config;

MqttServiceConfig _config = new()
{
    Port = 1883,
    Users = new List<User>
    {
        new()
        {
            UserName = "user1",
            Password = "password1"
        },
        new()
        {
            UserName = "user2",
            Password = "password2"
        }
    },
    DelayInMilliSeconds = 30000,
    TlsPort = 8883
};

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<MqttService>(provider =>
        {
            return new MqttService(_config, "MqttService");
        });
    })
    .Build();

host.Run();