using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.WaterManager.Reservations;

public class Index : PageModel
{
    private readonly IReservationRepository _reservations;
    
    public IEnumerable<Reservation> CurrentReservations { get; set; }

    public Index(IReservationRepository reservations)
    {
        _reservations = reservations;

        CurrentReservations = Enumerable.Empty<Reservation>();
    }
    
    public void OnGet(int waterCompanyId)
    {
        CurrentReservations =
            from reservation in _reservations.GetAllByWaterCompany(waterCompanyId).ToBlockingEnumerable()
            where reservation.OnGoing && reservation.Accepted
            select reservation;
    }
}