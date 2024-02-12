namespace RadiometerWebApp.Utils;

public static class TokenValidator
{
    public static bool IsTokenInvalid(ApplicationContext db, string token)
    {
        return db.Tokens.Any(x => x.Token == token 
                                  && (x.ExpirationDate < DateTime.UtcNow || x.Revoked == true));
    }
}