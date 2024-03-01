using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using mqtt_client;

namespace DATA_processor;

public class Processor : BackgroundService
{
    private readonly ILogger _logger;
    private DbHandler _dbHandler;

    public Processor(ILogger logger)
    {
        _logger = logger;
        _dbHandler = new DbHandler();
    }
    
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Information("Starting service");
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var messages = _dbHandler.GetMessages();
            if(messages.Count > 0)
            {
                ProcessMessages(messages);
            }
            
            _logger.Information("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
    
    public void ProcessMessages(List<BsonDocument> messages)
    {
        foreach (var message in messages)
        {
            var reading = BsonSerializer.Deserialize<DataModel>(message);

            var dataDTO = new DataDTO()
            {
                Timestamp = DateTime.Parse(reading.Time),
                TotalStartTime = DateTime.Parse(reading.ENERGY.TotalStartTime),
                Total = reading.ENERGY.Total,
                Today = reading.ENERGY.Today,
                Power = reading.ENERGY.Power,
                ApparentPower = reading.ENERGY.ApparentPower,
                ReactivePower = reading.ENERGY.ReactivePower,
                Factor = reading.ENERGY.Factor,
                Voltage = reading.ENERGY.Voltage,
                Current = reading.ENERGY.Current,
            };
            
            _dbHandler.InsertData(dataDTO);
            _dbHandler.RemoveMessage(message);
        }
    }
}