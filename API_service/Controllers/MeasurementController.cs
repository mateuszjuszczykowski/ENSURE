using DATABASE_library;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_service.Controllers;

[ApiController]
[Route("[controller]")]
public class MeasurementController : ControllerBase
{
    private readonly DbHandler _dbHandler;

    public MeasurementController(DbHandler handler)
    {
        _dbHandler = handler;
    }
    
    //todo: add auto close measurement when usage drops for a certain time
    
    [HttpGet("all", Name = "GetMeasurements")]
    public IActionResult GetMeasurements()
    {
        var measurements = _dbHandler.GetMeasurements();
        var response = JsonConvert.SerializeObject(measurements);
        return Ok(response);
    }
    
    [HttpGet("", Name = "GetMeasurement")]
    public IActionResult GetMeasurement([FromQuery] string measurementId)
    {
        var measurement = _dbHandler.GetMeasurement(measurementId);
        var response = JsonConvert.SerializeObject(measurement);
        return Ok(response);
    }
    
    [HttpPost("set", Name = "SetMeasurement")]
    public IActionResult SetMeasurement([FromQuery] string deviceId, string name, string category, DateTime start, DateTime end)
    {
        _dbHandler.SetMeasurement(deviceId, name, category, start, end);
        return Ok();
    }

    [HttpPost("start", Name = "StartMeasurement")]
    public IActionResult StartMeasurement([FromQuery] string deviceId, string name, string category)
    {
        _dbHandler.StartMeasurement(deviceId, name, category);
        return Ok();
    }

    [HttpPost("stop", Name = "StopMeasurement")]
    public IActionResult StopMeasurement([FromQuery] string deviceId)
    {
        _dbHandler.StopMeasurement(deviceId);
        return Ok();
    }

    [HttpPost("resume", Name = "ResumeMeasurement")]
    public IActionResult ResumeMeasurement([FromQuery] string deviceId)
    {
        _dbHandler.ResumeMeasurement(deviceId);
        return Ok();
    }

    [HttpPost("end", Name = "EndMeasurement")]
    public IActionResult EndMeasurement([FromQuery] string deviceId)
    {
        _dbHandler.EndMeasurement(deviceId);
        return Ok();
    }
    
    [HttpDelete("", Name = "DeleteMeasurement")]
    public IActionResult DeleteMeasurement([FromQuery] string measurementId)
    {
        _dbHandler.DeleteMeasurement(measurementId);
        return Ok();
    }
}