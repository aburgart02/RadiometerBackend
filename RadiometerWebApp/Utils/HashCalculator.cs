using System.Security.Cryptography;
using System.Text;

namespace RadiometerWebApp.Utils;

public static class HashCalculator
{
    private const int SaltLength = 32;
    
    public static string CalculateHash(string password, string salt)
    {
        var inputBytes = Encoding.UTF8.GetBytes(password + salt);
        var inputHash = SHA256.HashData(inputBytes);
        return Convert.ToHexString(inputHash);
    }
    
    public static string GenerateRandomString()
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var sb = new StringBuilder();
        
        using (var rng = new RNGCryptoServiceProvider())
        {
            var randomBytes = new byte[SaltLength];
            rng.GetBytes(randomBytes);
            
            foreach (var b in randomBytes)
            {
                sb.Append(validChars[b % validChars.Length]);
            }
        }
        
        return sb.ToString();
    }
}