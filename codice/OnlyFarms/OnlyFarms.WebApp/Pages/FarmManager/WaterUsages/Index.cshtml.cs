using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.FarmManager.WaterUsages;

[Authorize]     // questa pagina e' accessibile sia da utenti FarmManger sia da utenti WaterManager
public class Index : PageModel
{
    private readonly IWaterUsageRepository _waterUsages;

    //public IEnumerable<WaterUsage> WaterUsages { get; set; }
    public IEnumerable<(DateTime TimeStamp, int ConsumedQuantity)> WaterUsages { get; set; }
    public string? CompanyName { get; set; }

    public Index(IWaterUsageRepository waterUsages)
    {
        _waterUsages = waterUsages;
        WaterUsages = Enumerable.Empty<(DateTime, int)>();
    }

    /* spiegazione: string? companyName viene settato SOLO quando un WaterManager accede a questa pagina
     per vedere la cronologia di utilizzi di una delle FarmingCompany associate alla propria WaterCompany*/
    public async void OnGet(int farmingCompanyId, [FromQuery] string? companyName)
    {
        //WaterUsages = _waterUsages.GetAll(farmingCompanyId).ToBlockingEnumerable();
        WaterUsages = await _waterUsages.GetTotalWaterUsage(farmingCompanyId);
        CompanyName = companyName;
    }
}