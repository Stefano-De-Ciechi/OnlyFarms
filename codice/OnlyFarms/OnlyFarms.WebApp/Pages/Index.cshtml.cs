using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        // logica per avere pagine di "default" quando un certo tipo di utente esegue il login
        if (User.Identity != null && User.Identity.IsAuthenticated && User.HasClaim(nameof(Roles), Roles.Admin))
        {
            return RedirectToPage("/AdminPage");
        }

        if (User.Identity != null && User.Identity.IsAuthenticated && User.HasClaim(nameof(Roles), Roles.FarmManager))
        {
            return RedirectToPage("/FarmManager/Profile");
        }

        if (User.Identity != null && User.Identity.IsAuthenticated && User.HasClaim(nameof(Roles), Roles.WaterManager))
        {
            return RedirectToPage("/WaterManager/Profile");
        }

        return Page();
    }
}
