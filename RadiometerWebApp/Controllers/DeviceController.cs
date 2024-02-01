using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RadiometerWebApp.Models;
using RadiometerWebApp.Utils;

namespace RadiometerWebApp.Controllers;

public class DeviceController : Controller
{
    private ApplicationContext _db;
    
    public DeviceController(ApplicationContext context)
    {
        _db = context;
    }
    
    [HttpPost]
    [Route("add-device")]
    public IActionResult AddDevices(Device device)
    {
        if (!TokenValidator.IsTokenValid(_db, Request.Headers["Token"]))
            return Unauthorized();
        
        _db.Devices.Add(device);
        _db.SaveChanges();
        return Ok();
    }
    
    [HttpGet]
    [Route("devices")]
    public IActionResult GetDevices()
    {
        if (!TokenValidator.IsTokenValid(_db, Request.Headers["Token"]))
            return Unauthorized();
        
        var devices = _db.Devices.ToList();
        return Ok(JsonSerializer.Serialize(devices));
    }
}