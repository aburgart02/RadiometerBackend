using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [HttpPost]
    [Route("delete-user")]
    public IActionResult DeleteUser([FromBody] User user)
    {
        var dbUser = _db.Users.Include(x => x.Measurements)
            .FirstOrDefault(x => x.Id == user.Id);
        if (dbUser == null || dbUser.Measurements.Count > 0)
            return BadRequest();

        _db.Remove(dbUser);
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize(Roles = Role.Admin)]
    [HttpPut]
    [Route("update-user")]
    public IActionResult UpdateUser([FromBody] User user)
    {
        var dbUser = _db.Users.FirstOrDefault(x => x.Id == user.Id);
        if (dbUser == null)
            return BadRequest();

        dbUser.Login = user.Login;
        dbUser.Name = user.Name;
        dbUser.Surname = user.Surname;
        dbUser.Patronymic = user.Patronymic;
        dbUser.BirthDate = user.BirthDate?.ToUniversalTime();
        dbUser.Sex = user.Sex;
        dbUser.Role = user.Role;
        dbUser.Revoked = user.Revoked;
        dbUser.Notes = user.Notes;
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize(Roles = Role.Admin)]
    [HttpPost]
    [Route("update-user-password")]
    public IActionResult DeleteToken([FromBody] User user)
    {
        var dbUser = _db.Users.FirstOrDefault(x => x.Id == user.Id);
        if (dbUser == null)
            return BadRequest();
        
        var salt = HashCalculator.GenerateRandomString();
        dbUser.Salt = salt;
        dbUser.Password = HashCalculator.CalculateHash(user.Password, salt);
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
            Patronymic = x.Patronymic,
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