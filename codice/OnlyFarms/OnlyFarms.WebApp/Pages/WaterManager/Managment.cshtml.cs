using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages.WaterManager
{
	public class ManagmentModel : PageModel
    {
        private readonly DataContext _context;
        public IQueryable<Reservation> ReservationsList { get; set; }
        private readonly IReservationRepository _reservationRepository;

        [BindProperty]
        public Reservation? reservation { get; set; }


        public ManagmentModel([FromServices] DataContext context, IReservationRepository reservationRepository)
        {
            _context = context;

            _reservationRepository = reservationRepository;
            ReservationsList = Enumerable.Empty<Reservation>().AsQueryable();
        }


        public void OnGet()
        {
            ReservationsList =
               from reservation in _context.Reservations
               where reservation.Accepted == false
               select reservation;
        }


        public async Task<IActionResult> OnPostSubmit(int reservationId)
        {     

            var res = await _reservationRepository.GetById(reservationId);
            res!.Accepted = true;

            

            await _reservationRepository.Update(res.FarmingCompanyId, res.WaterCompanyId, res.Id, res);

            //_context.Reservations.Entry(res).CurrentValues.SetValues(res);
            //_context.Reservations.Update(res);
            //await _context.SaveChangesAsync();

            return RedirectToPage("/WaterManager/Profile");
        }
    }
}
