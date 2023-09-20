namespace OnlyFarms.Core.Data;

// TODO quando si implementa l'autenticazione, questa classe deve estendere IdentityDbContext !!!

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    { }

    // TODO FarmingCompanies e WaterCompanies possono essere trasformate in un unica tabella; impostare dei vincoli sul tipo di azienda all'interno delle prenotazioni, dei consumi e delle coltivazioni

    public DbSet<Actuator> Actuators => Set<Actuator>();
    public DbSet<Command> ActuatorsCommands => Set<Command>();
    public DbSet<Crop> Crops => Set<Crop>();
    public DbSet<FarmingCompany> FarmingCompanies => Set<FarmingCompany>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Sensor> Sensors => Set<Sensor>();
    public DbSet<Measurement> SensorsMeasurements => Set<Measurement>();
    public DbSet<WaterCompany> WaterCompanies => Set<WaterCompany>();
    public DbSet<WaterUsage> WaterUsages => Set<WaterUsage>();
}