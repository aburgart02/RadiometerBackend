namespace RadiometerWebApp.Models;

public class Device
{
    public Device()
    {
        
    }
    
    public int Id { get; set; }

    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public List<Measurement> Measurements { get; set; } = new();
    
    public List<CalibrationData> CalibrationDatas { get; set; } = new();
}