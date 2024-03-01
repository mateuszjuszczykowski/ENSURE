using Microsoft.AspNetCore.Mvc;

namespace DATA_viewer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataController : ControllerBase
{
    private readonly DbHandler _dbHandler;

    public DataController(DbHandler dbHandler)
    {
        _dbHandler = dbHandler;
    }

    [HttpGet]
    public IActionResult Get(DateTime startDate, DateTime endDate)
    {
        var data = _dbHandler.GetDataBetweenDates(startDate, endDate);
        return Ok(data);
    }
}