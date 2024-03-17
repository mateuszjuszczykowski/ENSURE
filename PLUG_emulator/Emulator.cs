using System.Text.Json;
using System.Text.Json.Serialization;
using PLUG_emulator.config;

namespace PLUG_emulator;

public class Emulator: BackgroundService
{
    private ILogger _logger {get; set;}
    public MqttClientConfig MqttClientConfig { get; set; }
    private static double BytesDivider => 1048576.0; // 1024 * 1024
    private readonly string _serviceName = "MQTT emulator";
    private IMqttClient Client;
    private DataGenerator _dataGenerator;
    
    
    public Emulator(MqttClientConfig mqttClientConfig, string serviceName)
    {
        this.MqttClientConfig = mqttClientConfig; 
        this._serviceName = serviceName;
        this._logger = LoggerConfig.GetLoggerConfiguration().CreateLogger();
        this._dataGenerator = new DataGenerator();
        
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
        Client = client;
       
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
            await sendPeriodicData(Client, stoppingToken);
            await Task.Delay(MqttClientConfig.DelayInMilliSeconds, stoppingToken);
        }
    }
    
    private Task InterceptMessageAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        if (args.ApplicationMessage.Topic.Contains("cmnd"))
        {
            HandleCommand(args);
        }
        else
        {
            HandleMessage(args);
        }
        return Task.CompletedTask;
    }
    
    private void HandleMessage(MqttApplicationMessageReceivedEventArgs args)
    {
        var payload = args.ApplicationMessage?.PayloadSegment is null ? null : Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);
    
        if(payload is null)
        {
            _logger.Warning("Payload is null");
            return;
        }
        
        _logger.Information(
            "Message: ClientId = {ClientId}, Topic = {Topic}, Payload = {Payload}, QoS = {Qos}, Retain-Flag = {RetainFlag}",
            args.ClientId,
            args.ApplicationMessage?.Topic,
            payload,
            args.ApplicationMessage?.QualityOfServiceLevel,
            args.ApplicationMessage?.Retain);
    }
    
    private void HandleCommand(MqttApplicationMessageReceivedEventArgs args)
    {
        var command = Encoding.UTF8.GetString(args.ApplicationMessage.PayloadSegment);
        _logger.Information("Received command: {Command}", command);

        // Respond with a sample message
        var response = new MqttApplicationMessageBuilder()
            .WithTopic("response_topic")
            .WithPayload("Sample response")
            .WithRetainFlag()
            .Build();

        Client.PublishAsync(response, CancellationToken.None);
    }
    
    private async Task sendPeriodicData(IMqttClient client, CancellationToken stoppingToken)
    {
        await Task.Delay(500, stoppingToken);
        var data = _dataGenerator.GenerateData();
        var payload = JsonSerializer.Serialize(data);
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("tele/tasmota/SENSOR")
            .WithPayload(payload)
            .WithRetainFlag()
            .Build();
        await client.PublishAsync(message, stoppingToken);
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