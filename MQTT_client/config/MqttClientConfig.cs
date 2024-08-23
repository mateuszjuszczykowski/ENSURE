namespace MQTT_client.config;

public class MqttClientConfig
{
    public int Port { get; set; } = 1883;
    
    public string Broker { get; set; } = Environment.GetEnvironmentVariable("MQTT") ?? "localhost";
    
    public string ClientId { get; set; } = "dotnet-client";
    
    public string UserName { get; set; } = "client";
    
    public string Password { get; set; } = "password";
    
    public int DelayInMilliSeconds { get; set; } = 10000; 
    
    public List<string> Topics { get; set; } = new()
    {
        "tele/tasmota/+/SENSOR", // gathers data from ALL tasmota devices
        "tele/tasmota/SENSOR" //emulator
        //"tasmota/discovery/*"
    };
}