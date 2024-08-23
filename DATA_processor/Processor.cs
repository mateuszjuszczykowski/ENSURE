using DATABASE_library;
using DATABASE_library.Models.Data;

namespace DATA_processor;

public class Processor : BackgroundService
{
    private readonly ILogger _logger;
    private DbHandler _dbHandler;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Processor(ILogger logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = scopeFactory;
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
            using var scope = _serviceScopeFactory.CreateScope();
            _dbHandler = new DbHandler(scope.ServiceProvider.GetRequiredService<AppDbContext>());
            var messages = _dbHandler.GetRawMessages();
            if(messages.Count > 0)
            {
                ProcessMessages(messages);
            }
            
            _logger.Information("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(5000, stoppingToken);
        }
    }
    
    public void ProcessMessages(List<RawDataModel> messages)
    {
        // fetch newe
        foreach (var message in messages)
        {
            var rawMessage = message;
            var reading = rawMessage.Payload;

            var dataDTO = new DataModel()
            {
                deviceID = rawMessage.DeviceID,
                Timestamp = DateTime.Parse(reading.Time).ToUniversalTime(),
                TotalStartTime = DateTime.Parse(reading.ENERGY.TotalStartTime).ToUniversalTime(),
                Total = reading.ENERGY.Total,
                Today = reading.ENERGY.Today,
                Power = reading.ENERGY.Power,
                ApparentPower = reading.ENERGY.ApparentPower,
                ReactivePower = reading.ENERGY.ReactivePower,
                Factor = reading.ENERGY.Factor,
                Voltage = reading.ENERGY.Voltage,
                Current = reading.ENERGY.Current,
            };
            
            var measurement = _dbHandler.GetCurrentMeasurement(dataDTO.deviceID);
            if(measurement!=null)
            {
                dataDTO.Measurement = measurement;
            }
            
            _dbHandler.InsertData(dataDTO, "DATA");
            _dbHandler.RemoveMessage(message, "RAW");
        }
    }
}