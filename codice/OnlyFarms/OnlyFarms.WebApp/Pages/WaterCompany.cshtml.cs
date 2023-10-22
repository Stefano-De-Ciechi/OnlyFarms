using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages
{
    public class WaterCompanyModel : PageModel
    {
        readonly ICompanyRepository<WaterCompany> _companyRespository;

        public WaterCompanyModel(ICompanyRepository<WaterCompany> companyRespository)
        {
            _companyRespository = companyRespository;
        }
        
        public IAsyncEnumerable<ICompany>? Companies { get; set; }
        
        public void OnGet()
        {
            Companies = _companyRespository.GetAll();
        }
    }
}