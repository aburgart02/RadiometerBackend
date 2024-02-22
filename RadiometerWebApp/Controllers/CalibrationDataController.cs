using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadiometerWebApp.Models;
using RadiometerWebApp.Utils;

namespace RadiometerWebApp.Controllers;

public class CalibrationDataController : Controller
{
    private ApplicationContext _db;
    
    public CalibrationDataController(ApplicationContext context)
    {
        _db = context;
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin}")]
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
            .Any(x => x.Name == name)) 
            return BadRequest();
        
        _db.CalibrationDatas.Add(calibration);
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin}")]
    [HttpPost]
    [Route("delete-calibration")]
    public IActionResult DeleteCalibration([FromBody] CalibrationData calibration)
    {
        var dbCalibration = _db.CalibrationDatas
            .FirstOrDefault(x => x.Id == calibration.Id);
        if (dbCalibration == null)
            return BadRequest();

        _db.Remove(dbCalibration);
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin}")]
    [HttpPut]
    [Route("update-calibration")]
    public IActionResult UpdateCalibration([FromBody] CalibrationData calibration)
    {
        var dbCalibration = _db.CalibrationDatas.FirstOrDefault(x => x.Id == calibration.Id);
        if (dbCalibration == null)
            return BadRequest();

        dbCalibration.Name = calibration.Name;
        dbCalibration.Description = calibration.Description;
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin},{Role.ApiUser}")]
    [HttpGet]
    [Route("calibrations")]
    public IActionResult GetCalibrationDatas()
    {
        if (TokenValidator.IsTokenInvalid(_db, Request.Headers["Authorization"]))
            return Unauthorized();
        
        var calibrations = _db.CalibrationDatas.ToList();
        return Ok(JsonSerializer.Serialize(calibrations));
    }
}