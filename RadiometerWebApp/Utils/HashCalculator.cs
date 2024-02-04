using System.Security.Cryptography;
using System.Text;
using RadiometerWebApp.Security;

namespace RadiometerWebApp.Utils;

public static class HashCalculator
{
    public static string CalculateHash(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input + SecurityData.Salt);
        var inputHash = SHA256.HashData(inputBytes);
        return Convert.ToHexString(inputHash);
    }
}