using Microsoft.AspNetCore.Mvc;
using RadiometerWebApp.Models;

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
    public IActionResult Index(int id)
    {
        var measurement = _db.Measurements.ToList().FirstOrDefault(x => x.Id == id);
        return View(measurement);
    }
    
    [HttpPost]
    [Route("upload-measurement")]
    public void UploadMeasurement()
    {
        var file = HttpContext.Request.Form.Files.GetFile("file");
        
        using var fileStream = file.OpenReadStream();
        var measurementFile = new byte[file.Length];
        var number = fileStream.Read(measurementFile, 0, (int)file.Length);

        var measurementData = HttpContext.Request.Form;
        var measurement = new Measurement() { 
            Surname = measurementData["surname"].ToString(), 
            Name = measurementData["name"].ToString(), 
            Patronymic = measurementData["patronymic"].ToString(),
            Time = DateTime.Parse(measurementData["time"].ToString()).ToUniversalTime(),
            Patient = measurementData["patient"].ToString(),
            Device = measurementData["device"].ToString(),
            Data = measurementFile,
            Description = measurementData["description"].ToString()
        };
        
        _db.Measurements.Add(measurement);
        _db.SaveChanges();
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