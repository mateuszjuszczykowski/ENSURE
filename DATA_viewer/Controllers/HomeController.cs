using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DATA_viewer.Models;
using DATABASE_library;
using MongoDB.Driver.Linq;

namespace DATA_viewer.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DbHandler _dbHandler;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        _dbHandler = new DbHandler();
    }

    public IActionResult Index()
    {
        var data = _dbHandler.GetAllData("DATA");
        var latest = data.FirstOrDefault(e => e.Timestamp == data.Max(e => e.Timestamp));
        ViewBag.Data = data;
        ViewBag.Latest = latest;
        
        return View();
    }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}