using Microsoft.AspNetCore.Authorization;
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
    
    [Authorize]
    [HttpGet]
    [Route("measurement/{id}")]
    public IActionResult GetMeasurement(int id)
    {
        var measurement = _db.Measurements.ToList().FirstOrDefault(x => x.Id == id);
        return View(measurement);
    }
    
    [Authorize]
    [Route("measurements")]
    public IActionResult GetMeasurements()
    {
        var measurements = _db.Measurements.ToList();
        return View(measurements);
    }
    
    [Authorize]
    [HttpPost]
    [Route("add-measurement")]
    public IActionResult UploadMeasurement()
    {
        if (TokenValidator.IsTokenInvalid(_db, Request.Headers["Authorization"]))
            return Unauthorized();
        
        var file = HttpContext.Request.Form.Files.GetFile("file");
        byte[] measurementFile;
        
        using (var memoryStream = new MemoryStream())
        {
            file.CopyToAsync(memoryStream);
            measurementFile = memoryStream.ToArray();
        }

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
    
    [Authorize]
    [HttpGet]
    [Route("measurement/download/{id}")]
    public IActionResult DownloadMeasurement(int id)
    {
        if (TokenValidator.IsTokenInvalid(_db, Request.Headers["Authorization"]))
            return Unauthorized();
        
        var data = _db.Measurements.ToList().FirstOrDefault(x => x.Id == id).Data;
        var content = new MemoryStream(data);
        var contentType = "application/octet-stream";
        var fileName = "data";
        return File(content, contentType, fileName);
    }
}