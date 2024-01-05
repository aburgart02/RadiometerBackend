using Microsoft.AspNetCore.Mvc;

namespace RadiometerWebApp.Controllers;

public class HomeController : Controller
{
    public HomeController()
    {
        
    }

    public IActionResult Index()
    {
        return View();
    }
}