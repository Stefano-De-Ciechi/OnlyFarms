using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages;

[Authorize(Policy = Roles.FarmManager)]
public class FarmManagerProfile : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICompanyRepository<FarmingCompany> _companyRepository;

    private ApplicationUser? _user;
    
    [BindProperty]
    public FarmingCompany Company { get; set; }

    public FarmManagerProfile(UserManager<ApplicationUser> userManager, ICompanyRepository<FarmingCompany> companyRepository)
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