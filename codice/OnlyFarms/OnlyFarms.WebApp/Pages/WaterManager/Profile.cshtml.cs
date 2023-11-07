using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.WaterManager;

[Authorize(Policy = Roles.WaterManager)]

public class Profile : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICompanyRepository<WaterCompany> _companyRepository;

    private ApplicationUser? _user;
    
    [BindProperty]
    public WaterCompany Company { get; set; }
    
    [BindProperty]
    public int NewWaterSupplyValue { get; set; }
    
    [BindProperty]
    public int NewGlobalWaterLimitValue { get; set; }

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

    public async Task<IActionResult> OnPostUpdateInformations(int waterCompanyId)
    {
        var currentCompanyValues = await _companyRepository.Get(waterCompanyId);
        currentCompanyValues.WaterSupply = NewWaterSupplyValue;
        currentCompanyValues.GlobalWaterLimit = NewGlobalWaterLimitValue;
        
        await _companyRepository.Update(waterCompanyId, currentCompanyValues);
        return RedirectToPage("./Profile");
    }
}