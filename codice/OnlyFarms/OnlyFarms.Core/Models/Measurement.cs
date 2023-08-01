using OnlyFarms.Data;
namespace OnlyFarms.Models;

public record Measurement : IHasId       // entita' Misurazione (relativa ai sensori)
{
    public int Id { get; set; }
    public required DateTime Timestamp { get; set; } = DateTime.Now;
    public required float Value { get; set; }
    public required string MeasuringUnit { get; set; }
    
    // riferimenti al sensore
    public int SensorId { get; set; }
    public Sensor Sensor { get; set; }
}