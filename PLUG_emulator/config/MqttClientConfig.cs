namespace PLUG_emulator.config;

public class MqttClientConfig
{
    public int Port { get; set; } = 1883;
    
    public string Broker { get; set; } = "localhost";
    
    public string ClientId { get; set; } = "tasmota-emulator";
    
    public string UserName { get; set; } = "client";
    
    public string Password { get; set; } = "password";
    
    public int DelayInMilliSeconds { get; set; } = 10000; 
    
    public List<string> Topics { get; set; } = new()
    {
        "tele/tasmota/SENSOR",
        "cmnd/tasmota/json",
        "cmnd/tasmota/status11"

        //"tele/tasmota/STATE"
    };
}