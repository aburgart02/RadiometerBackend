namespace RadiometerWebApp.Models;

public class Measurement
{
    public Measurement()
    {

    }

    public int Id { get; set; }
    
    public DateTime Time { get; set; }
    
    public byte[] Data { get; set; }
    
    public string? Description { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    

    public int PatientId { get; set; }
    public Patient Patient { get; set; }
    
    
    public int DeviceId { get; set; }
    public Device Device { get; set; }
}