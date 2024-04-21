using System.Runtime.InteropServices;
using DATABASE_library;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

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

    [HttpGet(Name = "GetAllData")]
    [Authorize]
    public IActionResult GetAllData([FromQuery]int? limit, [FromQuery]DateTime? startDate, [FromQuery]DateTime? endDate)
    {
        List<DataModel> data = _dbHandler.GetAllData("DATA");
        if (limit != null)
        {
            data = data.Take((int) limit).ToList();
        }
        if (startDate != null && endDate != null)
        {
            data = data.Where(d => d.Timestamp >= startDate && d.Timestamp <= endDate).ToList();
        }
        return Ok(data);
    }

    // [HttpPost("transfer/{id}")]
    // public async Task<IActionResult> TransferData(string id)
    // {
    //     var filter = Builders<DataModel>.Filter.Eq(d => d._id, new ObjectId(id));
    //     var update = Builders<DataModel>.Update.Set(d => d.State, "in transfer");
    //     await _dataCollection.UpdateOneAsync(filter, update);
    //
    //     var data = await _dataCollection.Find(filter).FirstOrDefaultAsync();
    //
    //     // Send data to client and wait for confirmation
    //     // ...
    //
    //     update = Builders<DataModel>.Update.Set(d => d.State, "transferred");
    //     await _dataCollection.UpdateOneAsync(filter, update);
    //
    //     return Ok();
    // }
}