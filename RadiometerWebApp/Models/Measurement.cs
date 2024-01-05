namespace RadiometerWebApp.Models;

public class Measurement
{
    public Measurement()
    {

    }

    public int Id { get; set; }

    public string Surname { get; set; }
    public string Name { get; set; }
    public string Patronymic { get; set; }

    public DateTime Time { get; set; }
    
    public string Patient { get; set; }
    
    public string Device { get; set; }

    public byte[] Data { get; set; }

    public string Description { get; set; }
}