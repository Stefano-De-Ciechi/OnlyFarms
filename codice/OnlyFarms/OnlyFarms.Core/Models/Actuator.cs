namespace OnlyFarms.Core.Models;

public record Actuator : ICropComponent      // entita' Attuatore
{
    public int Id { get; set; }
    public required string Tag { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]    // per poter avere il parametro rappresentato come stringa e non come int nei json della REST API (nel DB viene comunque salvato come int)
    public required ActuatorType ActuatorType { get; set; }
    
    public int FarmingCompanyId { get; set; }     // riferimento all'azienda agricola di appartenenza
    public int CropId { get; set; }     // riferimento alla coltivazione di appartenenza

    // riferimento a tutti i comandi ricevuti dall'attuatore
    public ICollection<Command> Commands { get; init; } = new List<Command>();
}