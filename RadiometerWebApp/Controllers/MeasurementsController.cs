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
        return View();
    }
}