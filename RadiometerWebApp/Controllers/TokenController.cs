using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RadiometerWebApp.Models;
using RadiometerWebApp.Token;

namespace RadiometerWebApp.Controllers;

public class TokenController : Controller
{
    private ApplicationContext _db;
    
    public TokenController(ApplicationContext context)
    {
        _db = context;
    }
    
    [Authorize(Roles = Role.Admin)]
    [HttpPost]
    [Route("add-token")]
    public IActionResult AddToken([FromBody] AuthorizationToken token)
    {
        var now = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            notBefore: now,
            claims: new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultRoleClaimType, Role.ApiUser)
            },
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LifetimeInMinutes)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        
        var authorizationToken = new AuthorizationToken()
        { 
            Token = encodedJwt, 
            EmissionDate = now, 
            ExpirationDate = token.ExpirationDate?.ToUniversalTime(), 
            Revoked = false,
            Description = token.Description
        };
        
        _db.Tokens.Add(authorizationToken);
        _db.SaveChanges();
        
        return Ok(encodedJwt);
    }
    
    [Authorize(Roles = Role.Admin)]
    [HttpGet]
    [Route("tokens")]
    public IActionResult GetTokens()
    {
        var tokens = _db.Tokens.ToList();
        return Ok(JsonSerializer.Serialize(tokens));
    }
    
    [Authorize(Roles = Role.Admin)]
    [HttpPut]
    [Route("update-token")]
    public IActionResult UpdateToken([FromBody] AuthorizationToken token)
    {
        var dbToken = _db.Tokens.FirstOrDefault(x => x.Id == token.Id);
        if (dbToken == null)
            return BadRequest("Token doesn't exist");
        
        dbToken.Revoked = token.Revoked;
        dbToken.Description = token.Description;
        _db.SaveChanges();
        return Ok();
    }
    
    [Authorize(Roles = Role.Admin)]
    [HttpPost]
    [Route("delete-token")]
    public IActionResult DeleteToken([FromBody] AuthorizationToken token)
    {
        var dbToken = _db.Tokens.FirstOrDefault(x => x.Id == token.Id);
        if (dbToken == null)
            return BadRequest("Token doesn't exist");

        _db.Tokens.Remove(dbToken);
        _db.SaveChanges();
        return Ok();
    }
}