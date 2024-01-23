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
    
    [HttpPost]
    [Route("add-calibration-data")]
    public void AddCalibrationData()
    {
        var file = HttpContext.Request.Form.Files.GetFile("file");
        
        using var fileStream = file.OpenReadStream();
        var calibrationFile = new byte[file.Length];

        var calibrationForm = HttpContext.Request.Form;
        var name = calibrationForm["name"].ToString();
        var deviceId = Convert.ToInt32(calibrationForm["deviceId"]);
        var calibration = new CalibrationData() {
            Name = name,
            Date = DateTime.Parse(calibrationForm["time"].ToString()).ToUniversalTime(),
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
    }
}