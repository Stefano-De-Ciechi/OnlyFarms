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
        // TODO aggiungere qui la logica per avere pagine di default in base al tipo di utente?
        if (User.Identity != null && User.Identity.IsAuthenticated && User.HasClaim(nameof(Roles), Roles.Admin))
        {
            return RedirectToPage("/AdminPage");
        }

        if (User.Identity != null && User.Identity.IsAuthenticated && User.HasClaim(nameof(Roles), Roles.FarmManager))
        {
            return RedirectToPage("/FarmManager/Profile");
        }

        return Page();
    }
}
