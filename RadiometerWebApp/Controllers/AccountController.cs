using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RadiometerWebApp.Models;
using RadiometerWebApp.Token;
using RadiometerWebApp.Utils;

namespace RadiometerWebApp.Controllers;

public class AccountController : Controller
{
    private ApplicationContext _db;
    
    public AccountController(ApplicationContext context)
    {
        _db = context;
    }

    [Authorize]
    [HttpGet]
    [Route("checkAuth")]
    public IActionResult CheckAuth()
    {
        return Ok();
    }
    
    [HttpPost]
    [Route("login")]
    public IActionResult Login([FromBody] Credentials credentials)
    {
        var userData = GetIdentity(credentials);
        if (userData == null)
        {
            return Unauthorized();
        }

        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            notBefore: now,
            claims: userData.Value.claimsIdentity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LifetimeInMinutes)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        var response = new
        {
            access_token = encodedJwt,
            userId = userData.Value.Id
        };
        
        Logger.AddLog(_db, "AccountController", "Authorization", $"{credentials.Login} зашёл в систему");
        return Ok(Json(response));
    }
    
    private (ClaimsIdentity claimsIdentity, int Id)? GetIdentity(Credentials credentials)
    {
        var user = _db.Users.FirstOrDefault(x => x.Login == credentials.Login);
        if (user != null)
        {
            if (HashCalculator.CalculateHash(credentials.Password, user.Salt) != user.Password)
            {
                return null;
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };
            ClaimsIdentity? claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return (claimsIdentity, user.Id);
        }
        
        return null;
    }
}