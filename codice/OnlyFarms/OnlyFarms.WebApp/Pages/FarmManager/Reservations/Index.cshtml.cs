using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.FarmManager.Reservations;

[Authorize(Policy = Roles.FarmManager)]
public class Index : PageModel
{

    private readonly IReservationRepository _reservation;

    public Index(IReservationRepository reservation)
    {
        _reservation = reservation;
    }

    public IAsyncEnumerable<Reservation>? Reservation { get; set; }
    public IAsyncEnumerable<Reservation>? PastReservations { get; set; }


    public void OnGet(int farmingCompanyId)
    {
        Reservation = _reservation.GetReservation(farmingCompanyId);
        PastReservations = _reservation.GetAll(farmingCompanyId);   
    }
}