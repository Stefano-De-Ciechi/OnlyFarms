using OnlyFarms.Core.Data;

namespace OnlyFarms.Core.Models;

public record WaterUsage : IHasId    // entita' Consumo (di risorse idriche, relativa ad un'azienda Agricola)
{
    public int Id { get; set; }
    public required DateTime Timestamp { get; set; } = DateTime.Now;
    public required float ConsumedQuantity { get; set; }
    
    public int FarmingCompanyId { get; set; }   // riferimento all'azienda agricola
}