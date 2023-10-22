using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages
{
    public class CropsModel : PageModel
    {
        private readonly ICropRepository _cropRepository;

        public CropsModel(ICropRepository cropRepository)
        {
            _cropRepository = cropRepository;
        }
        
        public IAsyncEnumerable<Crop>? Crops { get; set; }

        public void OnGet(int farmingCompanyId)
        {
            Crops = _cropRepository.GetAll(1);
        }
    }
}