using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RadiometerWebApp.AuthOptions;
using RadiometerWebApp.Models;

namespace RadiometerWebApp.Controllers;

public class AccountController : Controller
{
    private ApplicationContext _db;
    
    public AccountController(ApplicationContext context)
    {
        _db = context;
    }

    [HttpPost]
    [Route("login")]
    public IActionResult Login(Credentials credentials)
    {
        var identity = GetIdentity(credentials);
        if (identity == null)
        {
            return Unauthorized();
        }

        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            notBefore: now,
            expires: now.AddDays(TokenConfiguration.LifetimeInDays),
            claims: identity.Claims,
            signingCredentials: new SigningCredentials(AuthOptions.AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        var tokenValue = Json(encodedJwt).Value.ToString();
        var token = new AuthorizationToken()
            { 
                Token = tokenValue, 
                EmissionDate = now, 
                ExpirationDate = now.AddDays(TokenConfiguration.LifetimeInDays), 
                Revoked = false 
            };
        _db.Tokens.Add(token);
        _db.SaveChanges();
        return Ok(tokenValue);
    }
    
    private ClaimsIdentity? GetIdentity(Credentials credentials)
    {
        var user = _db.Users.FirstOrDefault(x => x.Login == credentials.Login && x.Password == credentials.Password);
        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, ((int)user.Role).ToString())
            };
            ClaimsIdentity? claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
        
        return null;
    }
}