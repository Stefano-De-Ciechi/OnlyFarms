using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages;

[AllowAnonymous]
public class NonActiveAccount : PageModel
{
    public void OnGet()
    {
        
    }
}