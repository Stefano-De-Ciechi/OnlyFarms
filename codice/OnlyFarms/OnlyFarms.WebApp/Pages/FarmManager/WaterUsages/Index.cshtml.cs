using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.FarmManager.WaterUsages;

public class Index : PageModel
{
    private readonly IWaterUsageRepository _waterUsages;

    //IEnumerable<WaterUsage> WaterUsages { get; set; }
    public IAsyncEnumerable<WaterUsage> WaterUsages { get; set; }

    public Index(IWaterUsageRepository waterUsages)
    {
        _waterUsages = waterUsages;
    }

    public void OnGet(int farmingCompanyId)
    {
        WaterUsages = _waterUsages.GetAll(farmingCompanyId);
    }
}