using OnlyFarms.Data;
namespace OnlyFarms.Models;

public record Sensor : IHasId       // entita' Sensore
{
    public Sensor()
    {
        Measurements = new List<Measurement>();
    }

    public int Id { get; set; }
    public required SensorType Type { get; set; }
    public required string Tag { get; set; }
    public ICollection<Measurement> Measurements { get; init; }

}