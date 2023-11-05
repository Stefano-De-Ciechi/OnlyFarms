using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.WaterManager.Reservations
{
	public class ManageModel : PageModel
    {
        private readonly IReservationRepository _reservations;
        
        public IEnumerable<Reservation> ReservationsList { get; set; }
        
        public ManageModel(IReservationRepository reservations)
        {
            _reservations = reservations;
            ReservationsList = Enumerable.Empty<Reservation>();
        }


        public void OnGet(int waterCompanyId)
        {
            ReservationsList =
                from reservation in _reservations.GetAllByWaterCompany(waterCompanyId).ToBlockingEnumerable()
                where reservation.Accepted == false
                select reservation;
        }
        
        public async Task<IActionResult> OnPostSubmit(int reservationId, int waterCompanyId)
        {
            var toAccept = await _reservations.AcceptReservation(reservationId);    // questo metodo si occupa di accettare la Reservation e renderla l'unica OnGoing tra le due compagnie (ma una FarmingCompany puo' avere piu' prenotazioni attive con diverse WaterCompany contemporaneamente)
            return RedirectToPage("./Index", new { waterCompanyId });
        }
    }
}
