using Microsoft.AspNetCore.Mvc;
using RadiometerWebApp.Models;
using RadiometerWebApp.Utils;

namespace RadiometerWebApp.Controllers;

public class MeasurementController : Controller
{
    private ApplicationContext _db;
    
    public MeasurementController(ApplicationContext context)
    {
        _db = context;
    }
    
    [HttpGet]
    [Route("measurement/{id}")]
    public IActionResult GetMeasurement(int id)
    {
        var measurement = _db.Measurements.ToList().FirstOrDefault(x => x.Id == id);
        return View(measurement);
    }
    
    [Route("measurements")]
    public IActionResult GetMeasurements()
    {
        var measurements = _db.Measurements.ToList();
        return View(measurements);
    }
    
    [HttpPost]
    [Route("add-measurement")]
    public IActionResult UploadMeasurement()
    {
        if (!TokenValidator.IsTokenValid(_db, Request.Headers["Token"]))
            return Unauthorized();
        
        var file = HttpContext.Request.Form.Files.GetFile("file");
        
        using var fileStream = file.OpenReadStream();
        var measurementFile = new byte[file.Length];

        var measurementData = HttpContext.Request.Form;
        var measurement = new Measurement() {
            Time = DateTime.Parse(measurementData["time"].ToString()).ToUniversalTime(),
            Data = measurementFile,
            Description = measurementData["description"].ToString(),
            UserId = Convert.ToInt32(measurementData["userId"]),
            PatientId = Convert.ToInt32(measurementData["patientId"]),
            DeviceId = Convert.ToInt32(measurementData["deviceId"]),
        };
        
        _db.Measurements.Add(measurement);
        _db.SaveChanges();

        return Ok();
    }
    
    [HttpGet]
    [Route("measurement/download/{id}")]
    public IActionResult DownloadMeasurement(int id)
    {
        var data = _db.Measurements.ToList().FirstOrDefault(x => x.Id == id).Data;
        var content = new MemoryStream(data);
        var contentType = "application/octet-stream";
        var fileName = "data";
        return File(content, contentType, fileName);
    }
}