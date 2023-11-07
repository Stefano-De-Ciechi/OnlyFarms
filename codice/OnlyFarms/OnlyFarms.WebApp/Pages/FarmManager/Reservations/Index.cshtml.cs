using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.FarmManager.Reservations;

[Authorize(Policy = Roles.FarmManager)]
public class Index : PageModel
{
    private readonly IReservationRepository _reservations;
    
    public IEnumerable<Reservation> CurrentReservations { get; set; }
    public IEnumerable<Reservation> PendingReservations { get; set; }
    public IEnumerable<Reservation> PastReservations { get; set; }
    
    [BindProperty]
    public int DeleteReservationId { get; set; }

    public Index(IReservationRepository reservations)
    {
        _reservations = reservations;
        
        CurrentReservations = Enumerable.Empty<Reservation>();
        PendingReservations = Enumerable.Empty<Reservation>();
        PastReservations = Enumerable.Empty<Reservation>();
    }
    
    public void OnGet(int farmingCompanyId)
    {
        CurrentReservations = _reservations.GetCurrentReservations(farmingCompanyId).ToBlockingEnumerable();
        
        // TODO le due query qui sotto potrebbero essere trasformate in metodi della repository
        PastReservations =
            from reservation in _reservations.GetAll(farmingCompanyId).ToBlockingEnumerable()
            where reservation.OnGoing == false && reservation.Accepted      // filtra solamente le prenotazioni NON attive
            select reservation;

        PendingReservations =
            from reservation in _reservations.GetAll(farmingCompanyId).ToBlockingEnumerable()
            where reservation.OnGoing == false && reservation.Accepted == false     // filtra le prenotazioni NON ancora accettate e quindi NON attive
            select reservation;
        
    }

    public async Task<IActionResult> OnPostDeleteReservation()
    {
        /* la prenotazione non viene eliminata dal DB, viene settato il flag OnGoing=false e finisce nella tabella history*/
        var reservation = await _reservations.GetById(DeleteReservationId);
        reservation!.OnGoing = false;

        await _reservations.Update(reservation.FarmingCompanyId, reservation.WaterCompanyId, DeleteReservationId, reservation);
        return RedirectToPage("./Index", new { reservation.FarmingCompanyId });
    }
}