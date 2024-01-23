using Microsoft.AspNetCore.Mvc;
using RadiometerWebApp.Models;

namespace RadiometerWebApp.Controllers;

public class UserController : Controller
{
    private ApplicationContext _db;
    
    public UserController(ApplicationContext context)
    {
        _db = context;
    }
    
    [HttpPost]
    [Route("add-user")]
    public void AddUser(User user)
    {
        _db.Users.Add(user);
        _db.SaveChanges();
    }
}