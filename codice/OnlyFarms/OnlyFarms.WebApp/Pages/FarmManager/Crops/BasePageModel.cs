using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.FarmManager.Crops;

public class BasePageModel : PageModel
{
    protected readonly ICropRepository _crops;

    public BasePageModel(ICropRepository crops)
    {
        _crops = crops;
    }
}