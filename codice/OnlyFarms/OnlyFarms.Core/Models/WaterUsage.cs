﻿using OnlyFarms.Data;
namespace OnlyFarms.Models;

public record WaterUsage : IHasId    // entita' Consumo (di risorse idriche, relativa ad un'azienda Agricola)
{
    public required int Id { get; set; }
    public required DateTime Timestamp { get; set; } = DateTime.Now;
    public required float ConsumedQuantity { get; set; }
}