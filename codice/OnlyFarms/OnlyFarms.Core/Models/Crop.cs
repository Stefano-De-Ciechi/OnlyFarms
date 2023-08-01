using System.Text.Json.Serialization;
using OnlyFarms.Core.Data;

namespace OnlyFarms.Core.Models;

public record Crop : IHasId     // entita' Coltivazione
{
    public int Id { get; set; }
    public required string Tag { get; set; }
    public required float SurfaceArea { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]    // per poter avere il parametro rappresentato come stringa e non come int nei json della REST API (nel DB viene comunque salvato come int)
    public required IrrigationType IrrigationType { get; set; }
    
    public required float WaterNeeds { get; set; }
    public required float IdealHumidity { get; set; }
    
    public int FarmingCompanyId { get; set; }   // riferimento all'azienda a cui appartiene

    // riferimenti a sensori ed attuatori
    public ICollection<Sensor> Sensors { get; init; } = new List<Sensor>();
    public ICollection<Actuator> Actuators { get; init; } = new List<Actuator>();
}