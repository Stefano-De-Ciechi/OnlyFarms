using System.ComponentModel.DataAnnotations.Schema;
using OnlyFarms.Data;
using OnlyFarms.Infrastructure;

namespace OnlyFarms.Models;

public record Actuator : IHasId      // entita' Attuatore
{
    public int Id { get; set; }
    public required string Tag { get; set; }
    
    /*
     * spiegazione: per non avere l'enum ActuatorType salvato come int nel database ma avere una sua rappresentazione come stringa
     * si specifica che:
     *      1) la property ActuatorType NON deve essere mappato nel database (questa e' la versione int)
     *      2) la property TypeString "rimpiazza" la colonna precedente, e il suo valore e' la conversione in stringa dell'enum
     *
     * seguito l'esempio di https://stackoverflow.com/questions/32542356/how-to-save-enum-in-database-as-string
     */
    [NotMapped]
    public required ActuatorType ActuatorType { get; set; }
    
    [Column("Type")]
    public string TypeString
    {
        get => ActuatorType.ToString();
        private set => ActuatorType = value.ParseEnum<ActuatorType>();
    }
    
    // riferimenti alla coltivazione di appartenenza
    public int CropId { get; set; }
    public Crop Crop { get; set; }
    
    // riferimento a tutti i comandi ricevuti dall'attuatore
    public ICollection<ActuatorCommand> Commands { get; init; } = new List<ActuatorCommand>();
}