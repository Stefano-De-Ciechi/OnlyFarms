using OnlyFarms.Data;
namespace OnlyFarms.Models;

public record ActuatorCommand : IHasId       // entita' Comando (riferito agli Attuatori)
{
    public int Id { get; set; }
    public required DateTime TimeStamp { get; set; } = DateTime.Now;
    public required string State { get; set; }      // TODO modificare nei diagrammi il tipo di State da boolean a string ?    // indica se l'attuatore è attivo o no

    // riferimenti all'attuatore di cui il comando fa parte
    public int ActuatorId { get; set; }
    public Actuator Actuator { get; set; }
}