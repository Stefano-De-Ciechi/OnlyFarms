using OnlyFarms.Data;
namespace OnlyFarms.Models;

public record Actuator : IHasId      // entita' Attuatore
{
    public Actuator()
    { 
        Commands = new List<ActuatorCommand>();
    }

    public required int Id { get; set; }
    public required ICollection<ActuatorCommand> Commands { get; set; }
}