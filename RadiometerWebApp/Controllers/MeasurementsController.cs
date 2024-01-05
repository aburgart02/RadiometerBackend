using Microsoft.AspNetCore.Mvc;

namespace RadiometerWebApp.Controllers;

public class MeasurementsController : Controller
{
    ApplicationContext _db;

    public MeasurementsController(ApplicationContext context)
    {
        _db = context;
    }
    
    [Route("measurements")]
    public IActionResult Index()
    {
        var measurements = _db.Measurements.ToList();
        return View(measurements);
    }
}