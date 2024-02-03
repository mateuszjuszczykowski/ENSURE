using mqtt_broker.config;

namespace mqtt_broker;

public class MqttService: BackgroundService
{
    private readonly ILogger _logger;
    private readonly string _serviceName;
    private static double BytesDivider => 1048576.0; // 1024 * 1024
    public MqttServiceConfig MqttServiceConfiguration { get; set; }
    
    public MqttService(MqttServiceConfig mqttServiceConfiguration, string serviceName)
    {
        this.MqttServiceConfiguration = mqttServiceConfiguration;
        this._serviceName = serviceName;

        this._logger = LoggerConfig.GetLoggerConfiguration(nameof(MqttService))
            .CreateLogger();
    }

    /// <inheritdoc cref="BackgroundService"/>
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!this.MqttServiceConfiguration.IsValid())
        {
            throw new Exception("The configuration is invalid");
        }

        this._logger.Information("Starting service");
        this.StartMqttServer();
        this._logger.Information("Service started");
        await base.StartAsync(cancellationToken);
    }

    /// <inheritdoc cref="BackgroundService"/>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                this.LogMemoryInformation();
                await Task.Delay(this.MqttServiceConfiguration.DelayInMilliSeconds, cancellationToken);
            }
            catch (Exception ex)
            {
                this._logger.Error("An error occurred: {Exception}", ex);
            }
        }
    }
    
    public Task ValidateConnectionAsync(ValidatingConnectionEventArgs args)
    {
        try
        {
            var currentUser = this.MqttServiceConfiguration.Users.FirstOrDefault(u => u.UserName == args.UserName);

            if (currentUser == null)
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                this.LogMessage(args, true);
                return Task.CompletedTask;
            }

            if (args.UserName != currentUser.UserName)
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                this.LogMessage(args, true);
                return Task.CompletedTask;
            }

            if (args.Password != currentUser.Password)
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                this.LogMessage(args, true);
                return Task.CompletedTask;
            }

            args.ReasonCode = MqttConnectReasonCode.Success;
            this.LogMessage(args, false);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this._logger.Error("An error occurred: {Exception}.", ex);
            return Task.FromException(ex);
        }
    }
    
    public Task InterceptSubscriptionAsync(InterceptingSubscriptionEventArgs args)
    {
        try
        {
            args.ProcessSubscription = true;
            this.LogMessage(args, true);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this._logger.Error("An error occurred: {Exception}.", ex);
            return Task.FromException(ex);
        }
    }
    
    public Task InterceptApplicationMessagePublishAsync(InterceptingPublishEventArgs args)
    {
        try
        {
            args.ProcessPublish = true;
            this.LogMessage(args);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this._logger.Error("An error occurred: {Exception}.", ex);
            return Task.FromException(ex);
        }
    }
    
    private void StartMqttServer()
    {
        var optionsBuilder = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(this.MqttServiceConfiguration.Port)
            .WithEncryptedEndpointPort(this.MqttServiceConfiguration.TlsPort);

        var mqttServer = new MqttFactory().CreateMqttServer(optionsBuilder.Build());
        mqttServer.ValidatingConnectionAsync += this.ValidateConnectionAsync;
        mqttServer.InterceptingSubscriptionAsync += this.InterceptSubscriptionAsync;
        mqttServer.InterceptingPublishAsync += this.InterceptApplicationMessagePublishAsync;
        mqttServer.StartAsync();
    }
    
    private void LogMessage(InterceptingSubscriptionEventArgs args, bool successful)
    {
        this._logger.Information(
            successful
                ? "New subscription: ClientId = {ClientId}, TopicFilter = {TopicFilter}"
                : "Subscription failed for clientId = {ClientId}, TopicFilter = {TopicFilter}",
            args.ClientId,
            args.TopicFilter);
    }
    
    private void LogMessage(InterceptingPublishEventArgs args)
    {
        var payload = args.ApplicationMessage?.PayloadSegment is null ? null : Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);

        this._logger.Information(
            "Message: ClientId = {ClientId}, Topic = {Topic}, Payload = {Payload}, QoS = {Qos}, Retain-Flag = {RetainFlag}",
            args.ClientId,
            args.ApplicationMessage?.Topic,
            payload,
            args.ApplicationMessage?.QualityOfServiceLevel,
            args.ApplicationMessage?.Retain);
    }

    private void LogMessage(ValidatingConnectionEventArgs args, bool showPassword)
    {
        if (showPassword)
        {
            this._logger.Information(
                "New connection: ClientId = {ClientId}, Endpoint = {Endpoint}, Username = {UserName}, Password = {Password}, CleanSession = {CleanSession}",
                args.ClientId,
                args.Endpoint,
                args.UserName,
                args.Password,
                args.CleanSession);
        }
        else
        {
            this._logger.Information(
                "New connection: ClientId = {ClientId}, Endpoint = {Endpoint}, Username = {UserName}, CleanSession = {CleanSession}",
                args.ClientId,
                args.Endpoint,
                args.UserName,
                args.CleanSession);
        }
    }
    
    private void LogMemoryInformation()
    {
        var totalMemory = GC.GetTotalMemory(false);
        var memoryInfo = GC.GetGCMemoryInfo();
        var divider = BytesDivider;
        Log.Information(
            "Heartbeat for service {ServiceName}: Total {Total}, heap size: {HeapSize}, memory load: {MemoryLoad}.",
            this._serviceName, $"{(totalMemory / divider):N3}", $"{(memoryInfo.HeapSizeBytes / divider):N3}", $"{(memoryInfo.MemoryLoadBytes / divider):N3}");
    }
    
}