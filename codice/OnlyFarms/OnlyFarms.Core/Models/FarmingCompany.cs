namespace OnlyFarms.Core.Models;

/* 
    NOTE:
    avremmo potuto usare l'ereditarietà tra le classi Azienda Agricola e Azienda Idrica, tuttavia
    abbiamo riscontrato dei problemi con entity framework; in particolare la tabella per azienda
    agricola NON veniva creata, e nella tabella azienda agricola compariva un attributo "discriminator"
*/
public class FarmingCompany : ICompany      // entita' Azienda Agricola
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string City { get; set; }       // TODO aggiungere citta' ai diagrammi UML
    public required string Address { get; set; }
    public int WaterSupply { get; set; } = 0;     // TODO capire se ha senso cosi' o se e' meglio assegnare all'interno della POST request (ri-aggiungere required in quel caso)
    public string UniqueCompanyCode { get; init; } = Guid.NewGuid().ToString();     // da usare durante la fase di registrazione (ogni utente inserisce il codice della propria azienda)
    
    // riferimenti a prenotazioni, consumi e coltivazioni
    public ICollection<Reservation> Reservations { get; init; } = new List<Reservation>();
    public ICollection<WaterUsage> WaterUsages { get; init; } = new List<WaterUsage>();    // consumi idrici
    public ICollection<Crop> Crops { get; init; } = new List<Crop>(); // coltivazioni
}