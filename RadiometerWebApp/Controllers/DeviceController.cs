using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin}")]
    [HttpPost]
    [Route("add-device")]
    public IActionResult AddDevices([FromBody] Device device)
    {
        if (_db.Devices.Any(x => x.Name == device.Name)) 
            return BadRequest();
        
        _db.Devices.Add(device);
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin}")]
    [HttpPut]
    [Route("update-device")]
    public IActionResult UpdateDevice([FromBody] Device device)
    {
        var dbDevice = _db.Devices.FirstOrDefault(x => x.Id == device.Id);
        if (dbDevice == null)
            return BadRequest();

        dbDevice.Name = device.Name;
        dbDevice.Description = device.Description;
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin}")]
    [HttpPost]
    [Route("delete-device")]
    public IActionResult DeleteDevice([FromBody] Device device)
    {
        var dbDevice = _db.Devices
            .Include(x => x.Measurements)
            .Include(x => x.CalibrationDatas)
            .FirstOrDefault(x => x.Id == device.Id);
        if (dbDevice == null || dbDevice.Measurements.Count > 0 || dbDevice.CalibrationDatas.Count > 0)
            return BadRequest();

        _db.Remove(dbDevice);
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin},{Role.ApiUser}")]
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