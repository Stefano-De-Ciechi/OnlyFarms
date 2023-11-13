using System.ComponentModel.DataAnnotations;

namespace OnlyFarms.Core.Models;

//[PrimaryKey(nameof(Timestamp), nameof(FarmingCompanyId), nameof(CropId))]   // un solo WaterUsage per Crop per FarmingCompany al giorno
public record WaterUsage : IHasId    // entita' Consumo (di risorse idriche, relativa ad un'azienda Agricola)
{
    public int Id { get; set; }
    public required int ConsumedQuantity { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public int FarmingCompanyId { get; set; }   // riferimento all'azienda agricola
    public int CropId { get; set; }
}