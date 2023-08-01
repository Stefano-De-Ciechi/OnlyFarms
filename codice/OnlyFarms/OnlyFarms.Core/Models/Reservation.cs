using OnlyFarms.Core.Data;

namespace OnlyFarms.Core.Models;

public record Reservation : IHasId       // entita' Prenotazione
{
    public int Id { get; set; }
    public required DateTime TimeStamp { get; set; } = DateTime.Now;
    public required float BookedQuantity { get; set; }
    public required float Price { get; set; }
    public required bool OnGoing { get; set; }      // indica se una prenotazione è correntemente attiva
    
    // riferimenti alle aziende (agricola e idrica)
    public int FarmingCompanyId { get; set; }
    public int WaterCompanyId { get; set; }
}