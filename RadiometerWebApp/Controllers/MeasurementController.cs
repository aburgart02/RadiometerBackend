using System.Text.Json;
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

    [Authorize(Roles = $"{Role.Researcher},{Role.Admin},{Role.ApiUser}")]
    [HttpGet]
    [Route("measurements")]
    public IActionResult GetMeasurements()
    {
        if (TokenValidator.IsTokenInvalid(_db, Request.Headers["Authorization"]))
            return Unauthorized();
        
        var measurements = _db.Measurements.Select(x => new
        {
            Id = x.Id,
            Time = x.Time,
            Description = x.Description,
            UserId = x.UserId,
            PatientId = x.PatientId,
            DeviceId = x.DeviceId
        }).ToList();
        return Ok(JsonSerializer.Serialize(measurements));
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin},{Role.ApiUser}")]
    [HttpGet]
    [Route("measurements/{id}")]
    public IActionResult DownloadFile(int id)
    {
        if (TokenValidator.IsTokenInvalid(_db, Request.Headers["Authorization"]))
            return Unauthorized();
        
        var measurement = _db.Measurements.FirstOrDefault(x => x.Id == id);
        if (measurement == null)
            return BadRequest();

        return File(measurement.Data, "application/octet-stream");
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin}")]
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
}