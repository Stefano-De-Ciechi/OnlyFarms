namespace OnlyFarms.RestApi.Data;

// TODO quando si implementa l'autenticazione, questa classe deve estendere IdentityDbContext !!!

/* 
    la classe DataContext e' stata inserita in questa posizione del progetto (OnlyFarms.RestApi/Data) per due motivi:
    
    1) tenendola nella cartella Data del progetto OnlyFarms.Core si otteneva un errore provando a generare le migrations (il messaggio era "No DbContext was found in assembly OnltFarms.Api. Ensure that you're using the correct assembly ...) 
    2) da un punto di vista logico, la Rest Api e' l'unica parte del sistema che puo'/deve accedere direttamente al DB, quindi avere le classi relative al DbContext nella ClassLibrary condivisa OnlyFarms.Core sembrava una scelta sbagliata

 */

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

    /*protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WaterCompany>().UseTpcMappingStrategy();        // bisogna specificare di creare una tabella separata rispetto a WaterCompany (che e' la classe padre)
        modelBuilder.Entity<WaterCompany>().ToTable("WaterCompanies");
        modelBuilder.Entity<FarmingCompany>().ToTable("FarmingCompanies");
    }*/
}