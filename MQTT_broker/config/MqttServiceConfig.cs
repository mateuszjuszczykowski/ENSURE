namespace mqtt_broker.config;

public class MqttServiceConfig
{
    public int Port { get; set; } = 1883;
    
    public List<User> Users { get; set; } = new(
        
        new List<User>
        {
            new User
            {
                UserName = "user1",
                Password = "password1"
            },
            new User
            {
                UserName = "user2",
                Password = "password2"
            }
        });
    
    public int DelayInMilliSeconds { get; set; } = 30000;
    
    public int TlsPort { get; set; } = 8883;
    
    public bool IsValid()
    {
        if (this.Port is <= 0 or > 65535)
        {
            throw new Exception("The port is invalid");
        }

        if (!this.Users.Any())
        {
            throw new Exception("The users are invalid");
        }

        if (this.DelayInMilliSeconds <= 0)
        {
            throw new Exception("The heartbeat delay is invalid");
        }

        if (this.TlsPort is <= 0 or > 65535)
        {
            throw new Exception("The TLS port is invalid");
        }

        return true;
    }
}