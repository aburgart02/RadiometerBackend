using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RadiometerWebApp.AuthOptions;

public static class AuthOptions
{
    const string Key = "secret_key_secret_key_secret_key_secret_key_secret_key";
    
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}