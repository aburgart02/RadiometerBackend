using Microsoft.AspNetCore.Mvc;
using RadiometerWebApp.Models;

namespace RadiometerWebApp.Controllers;

public class LogController : Controller
{
    private ApplicationContext _db;
    
    public LogController(ApplicationContext context)
    {
        _db = context;
    }
    
    [HttpPost]
    [Route("add-log")]
    public void AddLog(Log log)
    {
        _db.Logs.Add(log);
        _db.SaveChanges();
    }
}