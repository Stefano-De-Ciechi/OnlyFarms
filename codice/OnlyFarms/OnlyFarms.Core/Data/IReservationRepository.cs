using OnlyFarms.Core.Models;

namespace OnlyFarms.Core.Data;

public interface IReservationRepository
{
    IAsyncEnumerable<Reservation> GetAll(int farmingCompanyId, int waterCompanyId);
    Task<Reservation> Get(int farmingCompanyId, int waterCompanyId, int id);
    Task<Reservation> Add(int farmingCompanyId, int waterCompanyId, Reservation reservation);
    Task<Reservation> Update(int farmingCompanyId, int waterCompanyId, int id, Reservation updatedReservation);
}