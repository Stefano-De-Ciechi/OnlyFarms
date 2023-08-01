using OnlyFarms.Data;
namespace OnlyFarms.Models;

public record Reservation : IHasId       // entita' Prenotazione
{
    public int Id { get; set; }
    public required DateTime TimeStamp { get; set; } = DateTime.Now;
    public required float BookedQuantity { get; set; }
    public required float Price { get; set; }
    public required bool OnGoing { get; set; }      // indica se una prenotazione è correntemente attiva
    
    // riferimento alle aziende (agricola e idrica)
    public int FarmingCompanyId { get; set; }
    public FarmingCompany FarmingCompany { get; set; }
    
    public int WaterCompanyId { get; set; }
    public WaterCompany WaterCompany { get; set; }
}