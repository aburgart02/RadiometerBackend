using Microsoft.AspNetCore.Mvc;

namespace RadiometerWebApp.Controllers;

public class MeasurementsController : Controller
{
    [Route("measurements")]
    public IActionResult Index()
    {
        return View();
    }
}