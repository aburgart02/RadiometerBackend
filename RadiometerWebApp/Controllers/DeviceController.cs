using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RadiometerWebApp.Models;

namespace RadiometerWebApp.Controllers;

public class DeviceController : Controller
{
    private ApplicationContext _db;
    
    public DeviceController(ApplicationContext context)
    {
        _db = context;
    }
    
    [Authorize]
    [HttpPost]
    [Route("add-device")]
    public IActionResult AddDevices([FromBody] Device device)
    {
        _db.Devices.Add(device);
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize]
    [HttpGet]
    [Route("devices")]
    public IActionResult GetDevices()
    {
        var devices = _db.Devices.ToList();
        return Ok(JsonSerializer.Serialize(devices));
    }
}