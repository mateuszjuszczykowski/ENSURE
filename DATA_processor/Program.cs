using DATA_processor;
using DATA_processor.config;
using DATABASE_library;
using Microsoft.EntityFrameworkCore;

Log.Logger = DATA_processor.config.LoggerConfig.GetLoggerConfiguration().CreateLogger();
IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
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
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton(Log.Logger);
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
        services.AddHostedService<Processor>();
    })
    .Build();

host.Run();