using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlyFarms.WebApp.Pages;

namespace OnlyFarms.WebApp.Pages.FarmManager.Reservations
{
	public class ReservationModel : PageModel
    {
        readonly ICompanyRepository<WaterCompany> _companyRespository;
        private DataContext Context { get; }

        public ReservationModel(ICompanyRepository<WaterCompany> companyRespository, DataContext _context)
        {
            _companyRespository = companyRespository;
            this.Context = _context;
        }

        public IAsyncEnumerable<ICompany>? Companies { get; set; }
        public Reservation Reservation{ get; set; }
        public WaterCompany WaterCompany { get; set; }

        public void OnGet()
        {
            Companies = _companyRespository.GetAll();
        }

        public IActionResult OnPostResult(Reservation reservation)
        {
            this.Context.Reservations.Add(reservation);
            this.Context.SaveChanges();
            return RedirectToPage("./Index");
        }
    }
}
