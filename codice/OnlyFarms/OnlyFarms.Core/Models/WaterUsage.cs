﻿namespace OnlyFarms.Core.Models;

public record WaterUsage : IHasId    // entita' Consumo (di risorse idriche, relativa ad un'azienda Agricola)
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public required int ConsumedQuantity { get; set; }
    
    public int FarmingCompanyId { get; set; }   // riferimento all'azienda agricola
}