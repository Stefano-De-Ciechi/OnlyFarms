using System.ComponentModel.DataAnnotations.Schema;
using OnlyFarms.Data;
using OnlyFarms.Infrastructure;

namespace OnlyFarms.Models;

public record Crop : IHasId     // entita' Coltivazione
{
    public int Id { get; set; }
    public required string Tag { get; set; }
    public required float SurfaceArea { get; set; }
    
    /*
     * spiegazione: per non avere l'enum IrrigationType salvato come int nel database ma avere una sua rappresentazione come stringa
     * si specifica che:
     *      1) la property IrrigationType NON deve essere mappato nel database (questa e' la versione int)
     *      2) la property TypeString "rimpiazza" la colonna precedente, e il suo valore e' la conversione in stringa dell'enum
     *
     * seguito l'esempio di https://stackoverflow.com/questions/32542356/how-to-save-enum-in-database-as-string
     */
    [NotMapped]
    public required IrrigationType IrrigationType { get; set; }

    [Column("Type")]
    public string TypeString
    {
        get => IrrigationType.ToString();
        private set => IrrigationType = value.ParseEnum<IrrigationType>();
    }
    
    public required float WaterNeeds { get; set; }
    public required float IdealHumidity { get; set; }
    
    // riferimento all'azienda a cui appartiene
    public int FarmingCompanyId { get; set; }
    public FarmingCompany FarmingCompany { get; set; }
    
    // riferimenti a sensori ed attuatori
    public ICollection<Sensor> Sensors { get; init; } = new List<Sensor>();
    public ICollection<Actuator> Actuators { get; init; } = new List<Actuator>();
}