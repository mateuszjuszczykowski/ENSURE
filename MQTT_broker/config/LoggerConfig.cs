namespace mqtt_broker.config;

public class LoggerConfig
{
    public static LoggerConfiguration GetLoggerConfiguration(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException(nameof(type), "The type of logger must be given.");
        }

        // set up logging for data frame output
        return new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("LoggerType", type);
    }
}