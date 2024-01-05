using Microsoft.AspNetCore.Mvc;

namespace RadiometerWebApp.Controllers;

public class MeasurementController : Controller
{
    ApplicationContext _db;

    public MeasurementController(ApplicationContext context)
    {
        _db = context;
    }
    
    [HttpGet]
    [Route("measurement/{id}")]
    public IActionResult Index(int id)
    {
        return View();
    }
    
    [HttpPost]
    [Route("upload-measurement")]
    public void Index()
    {
        
    }
}