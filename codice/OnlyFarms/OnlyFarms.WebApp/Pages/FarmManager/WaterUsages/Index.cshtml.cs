using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.FarmManager.WaterUsages;

public class Index : PageModel
{
    public void OnGet(int farmingCompanyId)
    {
        Console.WriteLine($"FarmingCompanyId is {farmingCompanyId}");
    }
}