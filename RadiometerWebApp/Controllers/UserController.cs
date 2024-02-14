using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
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
    
    [Authorize(Roles = Role.Admin)]
    [HttpPost]
    [Route("add-user")]
    public IActionResult AddUser([FromBody] User user)
    {
        if (_db.Users.Any(x => x.Login == user.Login)) 
            return BadRequest();
        
        var salt = HashCalculator.GenerateRandomString();
        user.Salt = salt;
        user.Password = HashCalculator.CalculateHash(user.Password, salt);
        user.BirthDate = user.BirthDate?.ToUniversalTime();
        _db.Users.Add(user);
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize(Roles = Role.Admin)]
    [HttpGet]
    [Route("users")]
    public IActionResult GetUsers()
    {
        var users = _db.Users.Select(x => new
        {
            Id = x.Id,
            Name = x.Name,
            Surname = x.Surname,
            Patromymic = x.Patronymic,
            BirthDate = x.BirthDate,
            Sex = x.Sex,
            Notes = x.Notes,
            Login = x.Login,
            Role = x.Role,
            Revoked = x.Revoked
        }).ToList();
        return Ok(JsonSerializer.Serialize(users));
    }
}