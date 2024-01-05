using Microsoft.AspNetCore.Mvc;

namespace RadiometerWebApp.Controllers;

public class MeasurementController : Controller
{
    [Route("measurement/{id}")]
    public IActionResult Index(int id)
    {
        return View();
    }
}