using MQTT_client.config;

namespace mqtt_client;

public class MqttClientWorker: BackgroundService
{
    private ILogger _logger {get; set;}
    public MqttClientConfig MqttClientConfig { get; set; }
    private static double BytesDivider => 1048576.0; // 1024 * 1024
    private readonly string _serviceName = "MQTT Client";
    private DbHandler _dbHandler;
    
    public MqttClientWorker(MqttClientConfig mqttClientConfig, string serviceName)
    {
        this.MqttClientConfig = mqttClientConfig; 
        this._serviceName = serviceName;
        this._logger = LoggerConfig.GetLoggerConfiguration().CreateLogger();
        this._dbHandler = new DbHandler();
        
        _logger.Information("Starting service");
        
    }
    
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var options = new MqttClientOptionsBuilder()
            .WithClientId(MqttClientConfig.ClientId)
            .WithTcpServer(MqttClientConfig.Broker, MqttClientConfig.Port)
            .WithCredentials(MqttClientConfig.UserName, MqttClientConfig.Password)
            .WithCleanSession()
            .Build();
        
        var factory = new MqttFactory();
        var mqttClient = factory.CreateMqttClient();
        var client = mqttClient;
       
        try
        {
            var response = await client.ConnectAsync(options, CancellationToken.None);
            _logger.Information("MqttClient connected with result: {ResultCode}", response.ResultCode);
            foreach (var topic in MqttClientConfig.Topics)
            {
                await client.SubscribeAsync(topic, MqttQualityOfServiceLevel.AtMostOnce, CancellationToken.None);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
       
        client.ApplicationMessageReceivedAsync += InterceptMessageAsync;

        _logger.Information("Starting service");
        
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            LogMemoryInformation();
            await Task.Delay(MqttClientConfig.DelayInMilliSeconds, stoppingToken);
        }
    }
    
    private Task InterceptMessageAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        HandleMessage(args);
        return Task.CompletedTask;
    }
    
    private void HandleMessage(MqttApplicationMessageReceivedEventArgs args)
    {
        var payload = args.ApplicationMessage?.PayloadSegment is null ? null : Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);
        
        var topicParts = args.ApplicationMessage?.Topic.Split('/');
        var deviceId = topicParts.Length > 2 ? topicParts[2] : "unknown";
    
        if(payload is null)
        {
            _logger.Warning("Payload is null");
            return;
        }
        
        var payloadWithId = $"{{\"deviceId\":\"{deviceId}\",\"payload\":{payload}}}";

        
        _logger.Information(
            "Message: ClientId = {ClientId}, Topic = {Topic}, Payload = {Payload}, QoS = {Qos}, Retain-Flag = {RetainFlag}",
            args.ClientId,
            args.ApplicationMessage?.Topic,
            payloadWithId,
            args.ApplicationMessage?.QualityOfServiceLevel,
            args.ApplicationMessage?.Retain);
        _dbHandler.InsertMessage(payloadWithId);
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