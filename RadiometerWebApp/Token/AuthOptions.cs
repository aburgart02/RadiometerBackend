using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RadiometerWebApp.Token;

public static class AuthOptions
{
    public const string Issuer = "Server";
    public const string Audience = "Client";
    const string Key = "secret_key_secret_key_secret_key_secret_key_secret_key";
    public const int LifetimeInMinutes = 1440;
    
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}