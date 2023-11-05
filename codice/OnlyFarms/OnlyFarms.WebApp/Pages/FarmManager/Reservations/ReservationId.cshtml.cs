using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.FarmManager.Reservations
{
	public class ReservationIdModel : PageModel
    {
        private readonly ICompanyRepository<WaterCompany> _companies;
        private readonly IReservationRepository _reservations;
        
        [BindProperty]
        public Reservation Reservation { get; set; }
        
        public ReservationIdModel(ICompanyRepository<WaterCompany> companies, IReservationRepository reservations)
        {
            _companies = companies;
            _reservations = reservations;
        }
        
        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync(int farmingCompanyId, int waterCompanyId)
        {
            var result = _companies.Get(waterCompanyId).Result;
            
            if (!ModelState.IsValid || Reservation.BookedQuantity > result.WaterSupply)
            {
                TempData["ErrorMessage"] = "Quantity of water inserted is not available for this water company";
                return Page();
            }

            await _reservations.Add(farmingCompanyId, waterCompanyId, Reservation);
            return RedirectToPage("/FarmManager/Reservations/Index", new { farmingCompanyId });
        }
    }
}