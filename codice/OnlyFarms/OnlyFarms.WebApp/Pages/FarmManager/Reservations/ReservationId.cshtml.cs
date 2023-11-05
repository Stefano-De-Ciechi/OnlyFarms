using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Protocol.Core.Types;
using OnlyFarms.Core.Models;
using OnlyFarms.WebApp.Pages;

namespace OnlyFarms.WebApp.Pages.FarmManager.Reservations
{
	public class ReservationIdModel : PageModel
    {
        readonly ICompanyRepository<WaterCompany> _companyRespository;
        private IReservationRepository _Repository { get; }
        public Reservation Reservation { get; set; }
        


        public ReservationIdModel([FromServices] IReservationRepository repository, ICompanyRepository<WaterCompany> companyRespository)
        {
            _Repository = repository;
            _companyRespository = companyRespository;
        }


        public async void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync(int farmingCompanyId, int waterCompanyId, Reservation reservation)
        {
            WaterCompany result = _companyRespository.Get(waterCompanyId).Result;
            
            if (!ModelState.IsValid || reservation.BookedQuantity > result.WaterSupply)
            {
                TempData["ErrorMessage"] = "Quantity of water inserted is not available for this water company";
                return Page();
            }

            await _Repository.Add(farmingCompanyId, waterCompanyId, reservation);
            return RedirectToPage("/FarmManager/Profile");
        }
    }
}