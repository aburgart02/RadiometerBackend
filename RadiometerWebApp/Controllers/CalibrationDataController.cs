using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiometerWebApp.Models;

namespace RadiometerWebApp.Controllers;

public class CalibrationDataController : Controller
{
    private ApplicationContext _db;
    
    public CalibrationDataController(ApplicationContext context)
    {
        _db = context;
    }
    
    [Authorize]
    [HttpPost]
    [Route("add-calibration")]
    public IActionResult AddCalibrationData()
    {
        var file = HttpContext.Request.Form.Files.GetFile("file");
        byte[] calibrationFile;
        
        using (var memoryStream = new MemoryStream())
        {
            file.CopyToAsync(memoryStream);
            calibrationFile = memoryStream.ToArray();
        }

        var calibrationForm = HttpContext.Request.Form;
        var name = calibrationForm["name"].ToString();
        var deviceId = Convert.ToInt32(calibrationForm["deviceId"]);
        var calibration = new CalibrationData() {
            Name = name,
            Date = DateTime.Parse(calibrationForm["date"].ToString()).ToUniversalTime(),
            Data = calibrationFile,
            Description = calibrationForm["description"].ToString(),
            DeviceId = deviceId
        };

        if (_db.Devices
            .Include(x => x.CalibrationDatas)
            .FirstOrDefault(x => x.Id == deviceId)!.CalibrationDatas
            .All(x => x.Name != name))
        {
            _db.CalibrationDatas.Add(calibration);
            _db.SaveChanges();
        }

        return Ok();
    }
    
    [Authorize]
    [HttpGet]
    [Route("calibrations")]
    public IActionResult GetCalibrationDatas()
    {
        var calibrations = _db.CalibrationDatas.ToList();
        return Ok(JsonSerializer.Serialize(calibrations));
    }
}