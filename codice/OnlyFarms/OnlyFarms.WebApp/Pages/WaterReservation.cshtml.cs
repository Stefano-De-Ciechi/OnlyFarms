using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages
{
    public class WaterReservationModel : PageModel
    {
        readonly IReservationRepository _waterReservation;
        public IAsyncEnumerable<Reservation>? WaterReservation { get; set; }

        public WaterReservationModel(IReservationRepository waterReservation)
        {
            _waterReservation = waterReservation;
        }

        public void OnGet(int id)
        {
            WaterReservation = _waterReservation.GetWaterComp(id);
        }
    }
}