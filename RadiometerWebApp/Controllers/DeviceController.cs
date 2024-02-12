using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
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
    
    [Authorize]
    [HttpPost]
    [Route("add-device")]
    public IActionResult AddDevices([FromBody] Device device)
    {
        if (TokenValidator.IsTokenInvalid(_db, Request.Headers["Authorization"]))
            return Unauthorized();
        
        if (_db.Devices.Any(x => x.Name == device.Name)) 
            return BadRequest();
        
        _db.Devices.Add(device);
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize]
    [HttpGet]
    [Route("devices")]
    public IActionResult GetDevices()
    {
        if (TokenValidator.IsTokenInvalid(_db, Request.Headers["Authorization"]))
            return Unauthorized();
        
        var devices = _db.Devices.ToList();
        return Ok(JsonSerializer.Serialize(devices));
    }
}