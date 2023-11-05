using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.FarmManager.Reservations
{
	public class ReservationModel : PageModel
    {
        readonly ICompanyRepository<WaterCompany> _companyRespository;
        private DataContext Context { get; }
        private IReservationRepository _reservationRepository;

        public ReservationModel(ICompanyRepository<WaterCompany> companyRespository,IReservationRepository reservationRepository )
        {
            _companyRespository = companyRespository;
            _reservationRepository = reservationRepository;
        }

        public IEnumerable<ICompany> Companies { get; set; }
        public Reservation Reservation{ get; set; }
        public WaterCompany WaterCompany { get; set; }

        public void OnGet()
        {
            Companies = _companyRespository.GetAll().ToBlockingEnumerable();
        }

        public IActionResult OnPostResult(Reservation reservation)
        {
            _reservationRepository.Add(reservation.FarmingCompanyId, reservation.WaterCompanyId, reservation);
            return RedirectToPage("./Index");
        }
    }
}

/*
namespace OnlyFarms.WebApp.Pages.FarmManager.Reservations
   {
   public class ReservationModel : PageModel
   {
   private readonly ICompanyRepository<WaterCompany> _companyRepository;
   private readonly IReservationRepository _reservations;
   private DataContext Context { get; }
   
   public ReservationModel(ICompanyRepository<WaterCompany> companyRepository, IReservationRepository reservations)
   {
   _companyRepository = companyRepository;
   _reservations = reservations;
   }
   
   public IEnumerable<ICompany> Companies { get; set; }
   public Reservation Reservation{ get; set; }
   public WaterCompany WaterCompany { get; set; }
   
   public void OnGet()
   {
   // TODO aggiungere un filtro per citta'?
   Companies = _companyRepository.GetAll().ToBlockingEnumerable();
   }
   
   public IActionResult OnPostResult(Reservation reservation)
   {
   //this.Context.Reservations.Add(reservation);
   //this.Context.SaveChanges();
   _reservations.Add
   return RedirectToPage("./Index");
   }
   }
   }
*/