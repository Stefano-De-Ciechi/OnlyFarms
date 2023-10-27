using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace OnlyFarms.Core.Data;

//public class DataContext : IdentityDbContext<IdentityUser>
public class DataContext : IdentityDbContext<ApplicationUser>
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    { }
    
    public DbSet<Actuator> Actuators => Set<Actuator>();
    public DbSet<Command> ActuatorsCommands => Set<Command>();
    public DbSet<Crop> Crops => Set<Crop>();
    public DbSet<FarmingCompany> FarmingCompanies => Set<FarmingCompany>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Sensor> Sensors => Set<Sensor>();
    public DbSet<Measurement> SensorsMeasurements => Set<Measurement>();
    public DbSet<WaterCompany> WaterCompanies => Set<WaterCompany>();
    public DbSet<WaterUsage> WaterUsages => Set<WaterUsage>();
    public DbSet<WaterLimit> WaterLimits => Set<WaterLimit>();
}