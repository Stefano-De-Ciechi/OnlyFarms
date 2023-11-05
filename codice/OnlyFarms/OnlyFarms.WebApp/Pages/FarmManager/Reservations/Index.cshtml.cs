using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.FarmManager.Reservations;

[Authorize(Policy = Roles.FarmManager)]
public class Index : PageModel
{

    private readonly IReservationRepository _reservations;

    public Index(IReservationRepository reservations)
    {
        _reservations = reservations;
    }

    //public IAsyncEnumerable<Reservation>? Reservation { get; set; }
    //public IAsyncEnumerable<Reservation>? PastReservations { get; set; }
    
    public Reservation? CurrentReservation { get; set; }
    public IEnumerable<Reservation> PastReservations { get; set; }

    public async Task<IActionResult> OnGet(int farmingCompanyId)
    {
        //Reservation = _reservations.GetReservation(farmingCompanyId);
        //PastReservations = _reservations.GetAll(farmingCompanyId);   
        
        CurrentReservation = await _reservations.GetCurrentReservation(farmingCompanyId);
        PastReservations =
            from reservation in _reservations.GetAll(farmingCompanyId).ToBlockingEnumerable()
            where reservation.OnGoing == false && reservation.Accepted == true      // filtra solamente le prenotazioni NON attive
            select reservation;

        return Page();
    }
}