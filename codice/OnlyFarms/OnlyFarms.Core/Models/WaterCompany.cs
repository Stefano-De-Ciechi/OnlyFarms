using OnlyFarms.Data;
namespace OnlyFarms.Models;

public class WaterCompany : IHasId      // entita' Azienda Idrica
{
    public WaterCompany()
    {
        Reservations = new List<Reservation>();
    }

    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required ICollection<Reservation> Reservations { get; set; }
    public required float WaterSupply { get; set; }
    public required string UniqueCompanyCode { get; set; } = Guid.NewGuid().ToString();     // da usare durante la fase di registrazione (ogni utente inserisce il codice della propria azienda)
}