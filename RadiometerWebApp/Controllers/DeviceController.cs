using System.Text.Json;
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
    
    [HttpPost]
    [Route("add-device")]
    public void AddDevices(Device device)
    {
        _db.Devices.Add(device);
        _db.SaveChanges();
    }
    
    [HttpGet]
    [Route("devices")]
    public string GetDevices()
    {
        var devices = _db.Devices.ToList();
        return JsonSerializer.Serialize(devices);
    }
}