using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using DATABASE_library;
using DATABASE_library.Models.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace API_service.Controllers;

[ApiController]
[Route("[controller]")]
public class DataController: ControllerBase
{
    private readonly DbHandler _dbHandler;
    
    public DataController(DbHandler handler)
    {
        _dbHandler = handler;
    }

    [HttpGet(Name = "GetData")]
    // [Authorize]
    public async Task<IActionResult> GetData([FromQuery]string? deviceId, [FromQuery] string start, [FromQuery]string end)
    {
        if (string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
        {
            return BadRequest();
        }
        
        var startDate = DateTime.Parse(start);
        var endDate = DateTime.Parse(end);
        
       // specify kind of datetime to UTC
       startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
       endDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
        
        var data = _dbHandler.GetData("DATA", deviceId, startDate, endDate);

        string? response;
        if(data.Count == 0)
        {
            data = new List<DataModel>() { };
            response = JsonConvert.SerializeObject(data);
            return NotFound(response);
        }
        response = JsonConvert.SerializeObject(data);
        return Ok(response);
    }
    
    [HttpGet("all", Name = "GetAllData")]
    public async Task<IActionResult> GetAllData()
    {
        var data = _dbHandler.GetAllData("DATA").ToList();
        var response = JsonConvert.SerializeObject(data);
        return Ok(response);
    }
}