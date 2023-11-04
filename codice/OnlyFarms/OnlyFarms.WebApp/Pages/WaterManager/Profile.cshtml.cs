using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.WaterManager;

[Authorize(Policy = Roles.WaterManager)]

// TODO i file FarmManager e WaterManager Profile.cshtml.cs potrebbero essere resi generici
public class Profile : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICompanyRepository<WaterCompany> _companyRepository;

    private ApplicationUser? _user;
    
    [BindProperty]
    public WaterCompany Company { get; set; }

    public Profile(UserManager<ApplicationUser> userManager, ICompanyRepository<WaterCompany> companyRepository)
    {
        _userManager = userManager;
        _companyRepository = companyRepository;
    }
    
    public async Task<IActionResult> OnGet()
    {
        _user = await _userManager.GetUserAsync(User);

        var companyId = _user!.CompanyId ?? default(int);   // "cast" da Nullable<int> a int
        Company = await _companyRepository.Get(companyId);
        return Page();
    }
}