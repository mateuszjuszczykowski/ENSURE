namespace mqtt_client;

public static class LoggerConfig
{
    public static LoggerConfiguration GetLoggerConfiguration()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console();
    }
}