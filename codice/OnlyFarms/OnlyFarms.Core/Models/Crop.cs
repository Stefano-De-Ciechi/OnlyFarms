using OnlyFarms.Data;
namespace OnlyFarms.Models;

public record Crop : IHasId     // entita' Coltivazione
{
    
    public Crop()
    {
        Sensors = new List<Sensor>();
        Actuators = new List<Actuator>();
    }

    public required int Id { get; set; }
    public required float SurfaceArea { get; set; }
    public required IrrigationType IrrigationType { get; set; }
    public required float WaterNeeds { get; set; }
    public required float IdealHumidity { get; set; }
    public required ICollection<Sensor> Sensors { get; set; }
    public required ICollection<Actuator> Actuators { get; set; }
}