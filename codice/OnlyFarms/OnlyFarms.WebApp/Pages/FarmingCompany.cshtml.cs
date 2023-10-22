using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages
{
    public class FarmingCompanyModel : PageModel
    {
        private readonly ICompanyRepository<FarmingCompany> _companyRepository;
        
        public FarmingCompanyModel(ICompanyRepository<FarmingCompany> companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public IAsyncEnumerable<ICompany> FarmingComp { get; set; }
        
        public void OnGet()
        {
            FarmingComp = _companyRepository.GetAll();
        }
    }
}