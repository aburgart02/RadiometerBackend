using System.ComponentModel.DataAnnotations;

namespace RadiometerWebApp.Models;

public class Log
{
    public Log()
    {
        
    }
    
    [Key]
    public DateTime Time { get; set; }
    
    public string Source { get; set; }
    
    public string Type { get; set; }
    
    public string Body { get; set; }
}