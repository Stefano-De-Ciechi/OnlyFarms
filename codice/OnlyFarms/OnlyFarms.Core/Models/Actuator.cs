using OnlyFarms.Data;
namespace OnlyFarms.Models;

public record Actuator : IHasId      // entita' Attuatore
{
    public Actuator()
    { 
        Commands = new List<ActuatorCommand>();
    }

    public int Id { get; set; }
    public required string Tag { get; set; }
    public ICollection<ActuatorCommand> Commands { get; init; }
}