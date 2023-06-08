using OnlyFarms.Data;
namespace OnlyFarms.Models;

// TODO per qualche motivo, le migrazioni non sembrano creare questa tabella nel DB, in più nella tabella WaterCompany viene aggiunto in automatico un attributo "Discriminator" ??? forse non e' saggio usare l'ereditarieta'?
public class FarmingCompany : IHasId      // entita' Azienda Agricola
{
    public FarmingCompany()
    { 
        Crops = new List<Crop>();
        WaterUsages = new List<WaterUsage>();
        Reservations = new List<Reservation>();
    }

    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required ICollection<Reservation> Reservations { get; set; }
    public required float WaterSupply { get; set; }
    public required string UniqueCompanyCode { get; set; } = Guid.NewGuid().ToString();     // da usare durante la fase di registrazione (ogni utente inserisce il codice della propria azienda)

    public required ICollection<WaterUsage> WaterUsages { get; set; }    // consumi idrici
    public required ICollection<Crop> Crops { get; set; }   // coltivazioni
}