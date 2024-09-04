using DATABASE_library;
using DATABASE_library.Models.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_service.Controllers;

[ApiController]
[Route("[controller]")]
public class SummaryController: ControllerBase
{
 
    private readonly DbHandler _dbHandler;

    public SummaryController(DbHandler handler)
    {
        _dbHandler = handler;
    }
    
    [HttpGet(Name = "GetSummary")]
    public IActionResult GetSummary([FromQuery] string deviceId)
    {
        if (string.IsNullOrEmpty(deviceId))
        {
            deviceId = "9437412";
            // return BadRequest();
        }
        
        var data = _dbHandler.GetAllData("DATA").Where(d => d.deviceID == deviceId).ToList();
        var pricePerKwh = 0.8;
        Console.WriteLine(data.Count);
        var summary = ProcessData(data, pricePerKwh);
        
        string? response;
        if (summary.Count == 0)
        {
            summary = new List<SummaryModel>() { };
            response = JsonConvert.SerializeObject(summary);
            return NotFound(response);
        }
        response = JsonConvert.SerializeObject(summary);
        return Ok(response);
    }
    
    [HttpDelete(Name = "DeleteSummary")]
    public IActionResult DeleteSummary([FromQuery] string start, string end, string deviceId)
    {
        if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end) || string.IsNullOrEmpty(deviceId))
        {
            return BadRequest();
        }

        var startDate = DateTime.Parse(start);
        var endDate = DateTime.Parse(end);

        // specify kind of datetime to UTC
        startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
        endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);

        // Retrieve data between the specified dates
        var data = _dbHandler.GetData("DATA", deviceId, startDate, endDate);
        
        if(data.Any(e => e.MeasurementId != null))
        {
            _dbHandler.DeleteMeasurement(data.Where(e => e.MeasurementId != null).First().MeasurementId);
        }
        
        _dbHandler.DeleteData("DATA", deviceId, startDate, endDate);
        return Ok();
    }
    
    private List<SummaryModel> ProcessData(List<DataModel> data, double pricePerKwh)
    {
        var summaryModels = new List<SummaryModel>();
        SummaryModel currentRecord = null;
        DateTime? previousTimestamp = null;

        foreach (var record in data)
        {
            var category = record.Measurement?.Category ?? "Unclassified";
            var name = record.Measurement?.Name;

            if (currentRecord == null || currentRecord.Category != category)
            {
                if (currentRecord != null)
                {
                    currentRecord.EndTime = previousTimestamp ?? record.Timestamp;
                    currentRecord.Duration = currentRecord.EndTime - currentRecord.StartTime;
                    summaryModels.Add(currentRecord);
                }

                currentRecord = new SummaryModel()
                {
                    Category = category,
                    Name = name,
                    StartTime = record.Timestamp,
                    NumberOfRecords = 0,
                    TotalUsage = 0,
                    TotalPrice = 0
                };
            }

            if (previousTimestamp.HasValue)
            {
                var timespan = (record.Timestamp - previousTimestamp.Value).TotalSeconds;

                if (timespan > 60)
                {
                    if (currentRecord.Category == "Unclassified")
                    {
                        currentRecord.EndTime = previousTimestamp ?? record.Timestamp;
                        currentRecord.Duration = currentRecord.EndTime - currentRecord.StartTime;
                        summaryModels.Add(currentRecord);

                        currentRecord = new SummaryModel()
                        {
                            Category = category,
                            Name = name,
                            StartTime = record.Timestamp,
                            NumberOfRecords = 0,
                            TotalUsage = 0,
                            TotalPrice = 0
                        };
                    }
                    previousTimestamp = record.Timestamp;
                    continue;
                }

                if (record.Power > 0)
                {
                    var usage = (record.Power  * timespan) / (1000 * 3600); // Convert to kWh
                    var price = usage * pricePerKwh;

                    currentRecord.TotalUsage += usage;
                    currentRecord.TotalPrice += price;
                }
            }

            currentRecord.NumberOfRecords++;
            previousTimestamp = record.Timestamp;
        }

        if (currentRecord != null)
        {
            currentRecord.EndTime = previousTimestamp ?? data.Last().Timestamp;
            currentRecord.Duration = currentRecord.EndTime - currentRecord.StartTime;
            summaryModels.Add(currentRecord);
        }

        return summaryModels;
    }
    
}