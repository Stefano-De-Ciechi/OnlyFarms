using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.FarmManager.Reservations
{
	public class ReservationModel : PageModel
    {
        private readonly ICompanyRepository<WaterCompany> _companies;
        private IReservationRepository _reservationRepository;
        
        public IEnumerable<WaterCompany> Companies { get; set; }

        public ReservationModel(ICompanyRepository<WaterCompany> companies, IReservationRepository reservationRepository)
        {
            _companies = companies;
            _reservationRepository = reservationRepository;
            
            Companies = Enumerable.Empty<WaterCompany>();
        }
        
        public void OnGet()
        {
            // TODO aggiungere un filtro per citta'?
            Companies = _companies.GetAll().ToBlockingEnumerable();
        }
        
    }
}
