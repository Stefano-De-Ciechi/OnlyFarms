using Microsoft.AspNetCore.Mvc;

namespace OnlyFarms.WebApp.Pages.FarmManager.Crops
{
    public class CropsModel : BasePageModel
    {
        public IEnumerable<Crop> Crops { get; set; }
        public int FarmingCompanyId { get; set; }
        
        [BindProperty]
        public Crop Crop { get; set; }

        public CropsModel(ICropRepository crops)
            : base(crops)
        {
            Crops = Enumerable.Empty<Crop>();
        }
        
        public void OnGet(int farmingCompanyId)
        {
            FarmingCompanyId = farmingCompanyId;
            Crops = _crops.GetAll(farmingCompanyId).ToBlockingEnumerable();
        }

        public async Task<IActionResult> OnPostCrop(int farmingCompanyId)
        {
            await _crops.Add(farmingCompanyId, Crop);
            return RedirectToPage("./Index", new { farmingCompanyId });
        }
    }
}