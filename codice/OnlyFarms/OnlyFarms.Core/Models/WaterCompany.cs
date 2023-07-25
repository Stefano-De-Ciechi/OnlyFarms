using OnlyFarms.Data;
namespace OnlyFarms.Models;

public class WaterCompany : IHasId      // entita' Azienda Idrica
{
    public WaterCompany()
    {
        Reservations = new List<Reservation>();
    }

    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public ICollection<Reservation> Reservations { get; init; }
    public required float WaterSupply { get; set; }
    public string UniqueCompanyCode { get; init; } = Guid.NewGuid().ToString();     // da usare durante la fase di registrazione (ogni utente inserisce il codice della propria azienda)
}