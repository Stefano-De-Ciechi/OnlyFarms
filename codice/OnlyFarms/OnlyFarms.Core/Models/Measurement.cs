using OnlyFarms.Core.Data;

namespace OnlyFarms.Core.Models;

public record Measurement : IHasId       // entita' Misurazione (relativa ai sensori)
{
    public int Id { get; set; }
    public required DateTime Timestamp { get; set; } = DateTime.Now;
    public required float Value { get; set; }
    public required string MeasuringUnit { get; set; }
    
    public int SensorId { get; set; }   // riferimento al sensore
}