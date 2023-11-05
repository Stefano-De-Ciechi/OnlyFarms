namespace OnlyFarms.Core.Models;

public record Reservation : IHasId       // entita' Prenotazione
{
    // TODO aggiungere un campo booleano 'Confirmed' ?? es. un'azienda agricola esegue la POST di una prenotazione, poi l'azienda idrica in questione con una PUT la puo' marcare come Confirmed (e il campo OnGoing viene settato a true)
    // possibile approfondimento: come impedire ai gestori idrici di modificare valori tipo booked quantity o price quando eseguono una PUT (usando diversi "modelli" di classe che espongono pubblicamente solo sottoinsiemi di proprieta'):    https://stackoverflow.com/questions/5367287/disable-required-validation-attribute-under-certain-circumstances
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public required int BookedQuantity { get; set; }
    public required float Price { get; set; }
    public bool OnGoing { get; set; }      // indica se una prenotazione e' correntemente attiva
    public bool Accepted { get; set; }     // indica se una prenotazione e' stata accettata dalla WaterCompany
    
    // riferimenti alle aziende (agricola e idrica)
    public int FarmingCompanyId { get; set; }
    public int WaterCompanyId { get; set; }
}