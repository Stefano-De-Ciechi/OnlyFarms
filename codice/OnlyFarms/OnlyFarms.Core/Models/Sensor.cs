using OnlyFarms.Data;
namespace OnlyFarms.Models;

public record Sensor : IHasId       // entita' Sensore
{
    public Sensor()
    {
        Measurements = new List<Measurement>();
    }

    public required int Id { get; set; }
    public required SensorType Type { get; set; }
    public required ICollection<Measurement> Measurements { get; set; }

}