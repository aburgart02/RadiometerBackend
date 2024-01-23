namespace RadiometerWebApp.Models;

public class CalibrationData
{
    public CalibrationData()
    {
        
    }
    
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public DateTime Date { get; set; }
    
    public byte[] Data { get; set; }
    
    public string? Description { get; set; }
    
    public int DeviceId { get; set; }

    public Device Device { get; set; }
}