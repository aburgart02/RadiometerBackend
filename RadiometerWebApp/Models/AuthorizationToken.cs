namespace RadiometerWebApp.Models;

public class AuthorizationToken
{
    public AuthorizationToken()
    {
        
    }

    public int Id { get; set; }

    public string Token { get; set; }

    public DateTime EmissionDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public bool Revoked { get; set; }
    
    public string? Description { get; set; }
}