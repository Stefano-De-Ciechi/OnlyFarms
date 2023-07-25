using OnlyFarms.Data;
namespace OnlyFarms.Models;

/* 
    NOTE:
    avremmo potuto usare l'ereditarietà tra le classi Azienda Agricola e Azienda Idrica, tuttavia
    abbiamo riscontrato dei problemi con entity framework; in particolare la tabella per azienda
    agricola NON veniva creata, e nella tabella azienda agricola compariva un attributo "discriminator"
*/
public class FarmingCompany : IHasId      // entita' Azienda Agricola
{
    public FarmingCompany()
    { 
        Crops = new List<Crop>();
        WaterUsages = new List<WaterUsage>();
        Reservations = new List<Reservation>();
    }

    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public ICollection<Reservation> Reservations { get; init; }
    public required float WaterSupply { get; set; }
    public string UniqueCompanyCode { get; init; } = Guid.NewGuid().ToString();     // da usare durante la fase di registrazione (ogni utente inserisce il codice della propria azienda)
    public ICollection<WaterUsage> WaterUsages { get; init; }    // consumi idrici
    public ICollection<Crop> Crops { get; init; }   // coltivazioni
}