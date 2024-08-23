using DATABASE_library;
using Microsoft.EntityFrameworkCore;
using mqtt_client;
using MQTT_client.config;
using MqttClient = mqtt_client.MqttClientWorker;

Log.Logger = LoggerConfig.GetLoggerConfiguration().CreateLogger();
MqttClientConfig mqttClientConfig = new MqttClientConfig();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton(Log.Logger);
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
        services.AddHostedService<MqttClientWorker>(provider =>
        {
            return new MqttClientWorker(mqttClientConfig, "MQTT Client", provider.GetRequiredService<IServiceScopeFactory>());
        });
    })
    .ConfigureAppConfiguration(((context, builder) =>
    {
        var buildConfig = builder.Build();
        var connectionString = buildConfig.GetConnectionString("DefaultConnection");
        var envConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (!string.IsNullOrEmpty(envConnectionString))
        {
            connectionString = envConnectionString;
        }
        builder.AddInMemoryCollection(new Dictionary<string, string>
        {
            {"ConnectionStrings:DefaultConnection", connectionString}
        });
    }))
    .Build();

host.Run();