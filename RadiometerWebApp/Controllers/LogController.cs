using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RadiometerWebApp.Controllers;

public class LogController : Controller
{
    private ApplicationContext _db;
    
    public LogController(ApplicationContext context)
    {
        _db = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("logs")]
    public IActionResult GetLogs()
    {
        var logs = _db.Logs.ToList();
        return Ok(JsonSerializer.Serialize(logs));
    }
}