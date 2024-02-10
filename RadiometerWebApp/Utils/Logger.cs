using RadiometerWebApp.Models;

namespace RadiometerWebApp.Utils;

public static class Logger
{
    public static void AddLog(ApplicationContext db, string source, string type, string body)
    {
        var now = DateTime.UtcNow;
        var log = new Log() { Time = now, Source = source, Type = type, Body = body };
        db.Logs.Add(log);
        db.SaveChanges();
    }
}