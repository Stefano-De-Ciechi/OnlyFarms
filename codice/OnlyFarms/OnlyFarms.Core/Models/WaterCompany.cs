namespace OnlyFarms.Core.Models;

public class WaterCompany : ICompany      // entita' Azienda Idrica
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string City { get; set; }       // TODO aggiungere il campo citta' ai diagrammi UML
    public required string Address { get; set; }
    
    public required float WaterSupply { get; set; }
    public string UniqueCompanyCode { get; init; } = Guid.NewGuid().ToString();     // da usare durante la fase di registrazione (ogni utente inserisce il codice della propria azienda)
    
    // riferimento alle prenotazioni
    public ICollection<Reservation> Reservations { get; init; } = new List<Reservation>();
}