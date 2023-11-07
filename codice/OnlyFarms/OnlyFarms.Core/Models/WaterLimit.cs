using System.ComponentModel.DataAnnotations.Schema;

namespace OnlyFarms.Core.Models;

public class WaterLimit : IHasId
{
    public int Id { get; set; }
    public required int Limit { get; set; }
    
    /*
     * inizialmente avevo messo solo le reference agli oggetti di tipo FarmingCompany e WaterCompany,
     * ma ho notato che nelle richieste al DB con il metodo GetAll, tutti i campi FarmingCompanyId venivano lasciati null;
     * per questo ho aggiunto esplicitamente il campo Id con l'attributo ForeignKey (almeno viene restituito l'Id e da quello
     * si puo' eventualmente recuperare la FarmingCompany con un'altra richiesta al DB)
     */
    
    [ForeignKey("FarmingCompanyId")]
    public int FarmingCompanyId { get; set; }
    public FarmingCompany FarmingCompany { get; set; }
    
    [ForeignKey("WaterCompanyId")]
    public int WaterCompanyId { get; set; }
    public WaterCompany WaterCompany { get; set; }
}