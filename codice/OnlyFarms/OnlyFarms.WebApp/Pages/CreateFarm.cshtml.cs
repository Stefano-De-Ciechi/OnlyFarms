using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages
{
    public class CreateFarmModel : PageModel
    {
        private DataContext Context { get; }

        public CreateFarmModel(DataContext _context)
        {
            this.Context = _context;
        }

        public FarmingCompany FarmingCompany { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPostSubmit(FarmingCompany farmingCompany)
        {
            this.Context.FarmingCompanies.Add(farmingCompany);
            this.Context.SaveChanges();
            
            return RedirectToPage("./FarmingCompanies");
        }
    }
}