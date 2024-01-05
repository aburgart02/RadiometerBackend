using Microsoft.EntityFrameworkCore;
using RadiometerWebApp.Models;

namespace RadiometerWebApp;

public class ApplicationContext : DbContext
{
    public DbSet<Measurement> Measurements { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}