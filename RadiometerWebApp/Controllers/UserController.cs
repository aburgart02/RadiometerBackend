using Microsoft.AspNetCore.Mvc;
using RadiometerWebApp.Models;
using RadiometerWebApp.Utils;

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
    public IActionResult AddUser(User user)
    {
        if (_db.Users.Any(x => x.Login == user.Login)) 
            return BadRequest();
        
        var salt = HashCalculator.GenerateRandomString();
        user.Salt = salt;
        user.Password = HashCalculator.CalculateHash(user.Password, salt);
        _db.Users.Add(user);
        _db.SaveChanges();
        return Ok();
    }
}