using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RadiometerWebApp.Models;
using RadiometerWebApp.Utils;

namespace RadiometerWebApp.Controllers;

public class MeasurementController : Controller
{
    private const int MaxBufferSize = 100000000;
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
    [Route("measurements-with-data")]
    public IActionResult GetMeasurementsWithData()
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
            DeviceId = x.DeviceId,
            Data = x.Data
        }).ToList();
        return Ok(JsonSerializer.Serialize(measurements));
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin},{Role.ApiUser}")]
    [HttpGet]
    [Route("measurement/{id}")]
    public IActionResult GetMeasurement(int id)
    {
        if (TokenValidator.IsTokenInvalid(_db, Request.Headers["Authorization"]))
            return Unauthorized();
        
        var measurement = _db.Measurements
            .Where(x => x.Id == id)
            .Select(x => new
            {
                Id = x.Id,
                Time = x.Time,
                Description = x.Description,
                UserId = x.UserId,
                PatientId = x.PatientId,
                DeviceId = x.DeviceId
            })
            .FirstOrDefault();
        return Ok(JsonSerializer.Serialize(measurement));
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin},{Role.ApiUser}")]
    [HttpGet]
    [Route("measurement-with-data/{id}")]
    public IActionResult GetMeasurementWithData(int id)
    {
        if (TokenValidator.IsTokenInvalid(_db, Request.Headers["Authorization"]))
            return Unauthorized();
        
        var measurement = _db.Measurements
            .Where(x => x.Id == id)
            .Select(x => new
            {
                Id = x.Id,
                Time = x.Time,
                Description = x.Description,
                UserId = x.UserId,
                PatientId = x.PatientId,
                DeviceId = x.DeviceId,
                Data = x.Data
            })
            .FirstOrDefault();
        return Ok(JsonSerializer.Serialize(measurement));
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin},{Role.ApiUser}")]
    [HttpGet]
    [Route("download-measurement/{id}")]
    public IActionResult DownloadMeasurementFile(int id)
    {
        if (TokenValidator.IsTokenInvalid(_db, Request.Headers["Authorization"]))
            return Unauthorized();
        
        var measurement = _db.Measurements.FirstOrDefault(x => x.Id == id);
        if (measurement == null)
            return NotFound("Measurement doesn't exist");

        return File(measurement.Data, "application/octet-stream");
    }
    
    [Authorize(Roles = $"{Role.Researcher},{Role.Admin}")]
    [HttpPost]
    [Route("add-measurement")]
    public async Task<IActionResult> UploadMeasurement()
    {
        var buffer = new byte[MaxBufferSize];
        var file = await HttpContext.Request.Form.Files.GetFile("file").OpenReadStream().ReadAsync(buffer);
        
        var i = buffer.Length - 1;
        while(buffer[i] == 0)
            --i;
        var measurementFile = new byte[i+1];
        Array.Copy(buffer, measurementFile, i+1);
        
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