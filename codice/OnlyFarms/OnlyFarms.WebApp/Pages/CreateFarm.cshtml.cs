using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages
{
    public class CreateFarmModel : PageModel
    {
        private ICompanyRepository<FarmingCompany> _Repository { get; }

        //public CreateFarmModel(DataContext _context)
        public CreateFarmModel([FromServices] ICompanyRepository<FarmingCompany> repository)
        {
            _Repository = repository;
        }

        public FarmingCompany FarmingCompany { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostSubmit(FarmingCompany farmingCompany)
        {
            //this._Repository.FarmingCompanies.Add(farmingCompany);
            //this._Repository.SaveChanges();
            await _Repository.Add(farmingCompany);
            
            return RedirectToPage("./FarmingCompanies");
        }
    }
}