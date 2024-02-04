using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

    [HttpPost]
    [Route("login")]
    public IActionResult Login(Credentials credentials)
    {
        var userData = GetIdentity(credentials);
        if (userData == null)
        {
            return Unauthorized();
        }

        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            notBefore: now,
            expires: now.AddDays(TokenConfiguration.LifetimeInDays),
            claims: userData.Value.claimsIdentity.Claims,
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
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
        
        var response = new
        {
            token = tokenValue,
            userId = userData.Value.Id
        };
        
        return Ok(response);
    }
    
    private (ClaimsIdentity claimsIdentity, int Id)? GetIdentity(Credentials credentials)
    {
        var password = HashCalculator.CalculateHash(credentials.Password);
        var user = _db.Users.FirstOrDefault(x => x.Login == credentials.Login && x.Password == password);
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
            return (claimsIdentity, user.Id);
        }
        
        return null;
    }
}