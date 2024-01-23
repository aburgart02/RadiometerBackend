using Microsoft.EntityFrameworkCore;
using RadiometerWebApp.Models;

namespace RadiometerWebApp;

public class ApplicationContext : DbContext
{
    public DbSet<Measurement> Measurements { get; set; }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Patient> Patients { get; set; }
    
    public DbSet<Device> Devices { get; set; }
    
    public DbSet<CalibrationData> CalibrationDatas { get; set; }
    
    public DbSet<Log> Logs { get; set; }
    

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>().HasIndex(u => u.Login).IsUnique();
        builder.Entity<Device>().HasIndex(l => l.Name).IsUnique();
    }
}