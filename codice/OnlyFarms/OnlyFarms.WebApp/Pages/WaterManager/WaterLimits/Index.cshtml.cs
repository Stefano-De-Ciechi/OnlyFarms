using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.WaterManager.WaterLimits;

// TODO fare in modo che quando venga creata una prenotazione di acqua venga creato un waterlimit impostato a zero tra le aziende coinvolte, cosi' il limite compare in questa pagina e lo si puo' modificare
// TODO oppure implementare un form di creazione di un water limit che da un elenco faccia selezionare un'azienda per cui e' attiva una prenotazione e per cui NON esistano altri limiti creati nel DB (la prima idea credo sia piu' facile)

[Authorize(Policy = Roles.WaterManager)]
public class Index : PageModel
{
    private readonly ICompanyRepository<FarmingCompany> _farmingCompanies;      // da usare per ottenere info. sull'azienda a cui fa riferimento ciascun limite
    private readonly IWaterLimitRepository _waterLimits;
    
    public IEnumerable<WaterLimit> WaterLimits { get; set; }
    
    [BindProperty]
    public int LimitId { get; set; }
    
    [BindProperty]
    public int NewLimitValue { get; set; }
    
    public Index(ICompanyRepository<FarmingCompany> farmingCompanies, IWaterLimitRepository waterLimits)
    {
        _farmingCompanies = farmingCompanies;
        _waterLimits = waterLimits;
    }
    
    public async Task<IActionResult> OnGet(int waterCompanyId)
    {
        WaterLimits = _waterLimits.GetAll(waterCompanyId).ToBlockingEnumerable();

        foreach (var limit in WaterLimits)
        {
            limit.FarmingCompany = await _farmingCompanies.Get(limit.FarmingCompanyId);
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostLimitUpdate(int waterCompanyId)
    {
        await _waterLimits.Update(LimitId, NewLimitValue);
        return RedirectToPage("./Index", new { waterCompanyId });
    }
}