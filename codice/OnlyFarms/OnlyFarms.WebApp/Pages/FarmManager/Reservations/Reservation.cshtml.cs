using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.FarmManager.Reservations
{
    [Authorize(Policy = Roles.FarmManager)]
	public class ReservationModel : PageModel
    {
        private readonly ICompanyRepository<WaterCompany> _companies;
        private readonly IReservationRepository _reservations;
        
        [BindProperty]
        public Reservation Reservation { get; set; }
        
        public IEnumerable<WaterCompany> Companies { get; set; }

        public ReservationModel(ICompanyRepository<WaterCompany> companies, IReservationRepository reservations)
        {
            _companies = companies;
            _reservations = reservations;
            
            Companies = Enumerable.Empty<WaterCompany>();
        }
        
        public void OnGet()
        {
            // TODO aggiungere un filtro per citta'?
            Companies = _companies.GetAll().ToBlockingEnumerable();
        }
        
        public async Task<IActionResult> OnPostReservation()
        {
            var farmingCompanyId = Reservation.FarmingCompanyId;
            var waterCompanyId = Reservation.WaterCompanyId;
            
            var result = _companies.Get(waterCompanyId).Result;
            
            if (!ModelState.IsValid || Reservation.BookedQuantity > result.WaterSupply)     // TODO qui bisognerebbe anche testare se esiste un WaterLimit e confrontarlo con la quantita' richiesta
            {
                // se si inseriscono valori non validi per la quantita' si viene reindirizzati alla stessa pagina ma impostando nella query dei valori di errore che verranno mostrati con una sorta di Alert di Bootstrap
                return RedirectToPage("/FarmManager/Reservations/Reservation", new { farmingCompanyId, errors = true, errorMessage = "The quantity of water inserted is not available for this water company" });
            }

            await _reservations.Add(farmingCompanyId, waterCompanyId, Reservation);
            return RedirectToPage("/FarmManager/Reservations/Index", new { farmingCompanyId });
        }
        
    }
}
