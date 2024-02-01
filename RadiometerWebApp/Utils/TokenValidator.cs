namespace RadiometerWebApp.Utils;

public static class TokenValidator
{
    public static bool IsTokenValid(ApplicationContext db, string token)
    {
        return db.Tokens.Any(x => x.Token == token && DateTime.UtcNow < x.ExpirationDate);
    }
}