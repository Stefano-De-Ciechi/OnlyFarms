using System.ComponentModel.DataAnnotations.Schema;
using OnlyFarms.Core.Data;

namespace OnlyFarms.Core.Models;

public record Command : IHasId, ICropComponentProperty       // entita' Comando (riferito agli Attuatori)
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public required string State { get; set; }      // TODO modificare nei diagrammi il tipo di State da boolean a string ?    // indica se l'attuatore è attivo o no

    /*
     * Spiegazione: nella REST API, gli endpoint di Command e Measurement sono molto simili tra loro, ma uno agisce
     * sui sensori mentre l'altro sugli attuatori; per poter scrivere un'unica repository generica e' necessario avere un modo
     * generico per accedere agli ID dei sensori o degli attuatori, che e' lo scopo della proprieta' ComponentId
     * (ho provato in molti modi con combinazioni di attributi come [ForeignKey("...")], [Column("...")] e/o [NotMapped],
     * ma non riesco a trovarne uno in cui nel DB compaia solamente la colonna ActuatorId e che si "mappi" correttamente
     * con ComponentId quando si eseguono delle richieste HTTP, quindi per ora bisogna mantenere la ridondanza delle due colonne)
     */
    public int ComponentId
    {
        get => ActuatorId;
        set => ActuatorId = value;
    }
    
    public int ActuatorId { get; set; }
}