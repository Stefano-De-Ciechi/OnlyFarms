using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.WaterManager.Reservations
{
	public class ManageModel : PageModel
    {
        private readonly IReservationRepository _reservations;
        private readonly IWaterLimitRepository _limits;
        
        public IEnumerable<Reservation> ReservationsList { get; set; }
        
        public ManageModel(IReservationRepository reservations, IWaterLimitRepository limits)
        {
            _reservations = reservations;
            _limits = limits;
            
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
            
            // quando una prenotazione viene accettata si crea in automatico un WaterLimit tra le due aziende (se non ne era gia stato creato uno per una prenotazione precedente)
            try
            {
                var previousLimit = await _limits.Get(waterCompanyId, toAccept.FarmingCompanyId);       // se non esiste nessun limite lancia NotFoundException<WaterLimit>
            }
            catch (NotFoundException<WaterLimit> e)
            {
                await _limits.Add(waterCompanyId, toAccept.FarmingCompanyId, 0);        // il limite iniziale e' settato a zero, modificabile poi dalla pagina Water Limits
            }
            
            return RedirectToPage("./Index", new { waterCompanyId });
        }
    }
}
