namespace OnlyFarms.Core.Data;

public interface IReservationRepository
{
    IAsyncEnumerable<Reservation> GetAll(int farmingCompanyId);
    Task<Reservation?> GetCurrentReservation(int farmingCompanyId);
    Task<Reservation?> GetById(int reservationId);
    IAsyncEnumerable<Reservation> GetAll(int farmingCompanyId, int waterCompanyId, DateTime? between, DateTime? and);
    Task<Reservation> Get(int farmingCompanyId, int waterCompanyId, int id);
    Task<Reservation> Add(int farmingCompanyId, int waterCompanyId, Reservation reservation);
    Task<Reservation> Update(int farmingCompanyId, int waterCompanyId, int id, Reservation updatedReservation);
    IAsyncEnumerable<Reservation> GetWaterComp(int waterCompanyId);
}