namespace RadiometerWebApp.Models;

public class User
{
    public User()
    {
        
    }
    
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public string Surname { get; set; }
    
    public string? Patronymic { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    public Sex? Sex { get; set; }
    
    public string? Notes { get; set; }
    
    public string Login { get; set; }
    
    public string Password { get; set; }
    
    public Role Role { get; set; }
    
    public bool Revoked { get; set; }
    
    public List<Measurement> Measurements { get; set; } = new();
}