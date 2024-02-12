namespace RadiometerWebApp.Utils;

public static class TokenValidator
{
    public static bool IsTokenInvalid(ApplicationContext db, string token)
    {
        var tokenValue = "";
        try
        {
            tokenValue = token.Split()[1];
        }
        catch (IndexOutOfRangeException e)
        {
            return true;
        }
        return db.Tokens.Any(x => x.Token == tokenValue && x.Revoked == true);
    }
}