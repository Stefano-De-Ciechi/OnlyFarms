namespace OnlyFarms.Core.Data;

public class ReservationRepository : IReservationRepository
{
    private readonly DataContext _context;
    private readonly DbSet<Reservation> _reservations;
    private readonly ICompanyRepository<FarmingCompany> _farmingCompanies;
    private readonly ICompanyRepository<WaterCompany> _waterCompanies;

    public ReservationRepository(DataContext context, ICompanyRepository<FarmingCompany> farmingCompanies, ICompanyRepository<WaterCompany> waterCompanies)
    {
        _context = context;
        _reservations = _context.Set<Reservation>();
        _farmingCompanies = farmingCompanies;
        _waterCompanies = waterCompanies;
    }

    public async IAsyncEnumerable<Reservation> GetAll(int farmingCompanyId)
    {
        var farmingCompany = await _farmingCompanies.Get(farmingCompanyId);
        foreach (var r in _reservations.Where(r => r.FarmingCompanyId == farmingCompany.Id)) yield return r;
    }

    public async IAsyncEnumerable<Reservation> GetReservation(int farmingCompanyId)
    {
        var farmingCompany = await _farmingCompanies.Get(farmingCompanyId);
        foreach (var r in _reservations.Where(r => r.FarmingCompanyId == farmingCompany.Id && r.OnGoing == true)) yield return r;
    }

    public async Task<Reservation?> GetById(int reservationId)
    {
        return await _reservations.FirstOrDefaultAsync(r => r.Id == reservationId && r.Accepted == false);
        
    }

    public async IAsyncEnumerable<Reservation> GetAll(int farmingCompanyId, int waterCompanyId, DateTime? between, DateTime? and)
    {
        and ??= DateTime.Now;
        
        var farmingCompany = await _farmingCompanies.Get(farmingCompanyId);
        var waterCompany = await _waterCompanies.Get(waterCompanyId);
        foreach (var r in _reservations.Where(r => r.FarmingCompanyId == farmingCompany.Id && r.WaterCompanyId == waterCompany.Id && r.Timestamp >= between && r.Timestamp <= and)) yield return r;
    }

    public async Task<Reservation> Get(int farmingCompanyId, int waterCompanyId, int id)
    {
        var farmingCompany = await _farmingCompanies.Get(farmingCompanyId);
        var waterCompany = await _waterCompanies.Get(waterCompanyId);
        
        var res = await _reservations.FirstOrDefaultAsync(r => r.Id == id && r.FarmingCompanyId == farmingCompany.Id && r.WaterCompanyId == waterCompany.Id);
        if (res == null)
        {
            throw new NotFoundException<Reservation>(id);
        }

        return res;
    }

    public async Task<Reservation> Add(int farmingCompanyId, int waterCompanyId, Reservation reservation)
    {
        var farmingCompany = await _farmingCompanies.Get(farmingCompanyId);
        var waterCompany = await _waterCompanies.Get(waterCompanyId);
        
        var previous = await _reservations.FirstOrDefaultAsync(r => r.OnGoing && r.FarmingCompanyId == farmingCompany.Id && r.WaterCompanyId == waterCompany.Id);
        if (previous != null)
        {
            previous.OnGoing = false;   // se si trova una prenotazione attiva viene resa inattiva
        }

        // TODO rivedere sta parte se si decide di aggiungere il campo Confirmed alle Reservations (dopo la POST di un' azienda agricola deve essere confermata dall' azienda idrica per diventare attiva e rimpiazzare la Reservation precedente)
        reservation.OnGoing = true;
        reservation.FarmingCompanyId = farmingCompanyId;
        reservation.WaterCompanyId = waterCompanyId;
        
        await _reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();

        return reservation;
    }

    public async Task<Reservation> Update(int farmingCompanyId, int waterCompanyId, int id, Reservation updatedReservation)
    {
        var farmingCompany = await _farmingCompanies.Get(farmingCompanyId);
        var waterCompany = await _waterCompanies.Get(waterCompanyId);
        
        var res = await Get(farmingCompany.Id, waterCompany.Id, id);

        if (updatedReservation.OnGoing && _reservations.Any(r => !r.OnGoing && r.FarmingCompanyId == farmingCompany.Id && r.WaterCompanyId == waterCompany.Id))
        {
            throw new DbUpdateException("can't have more than one active Reservations between the same companies at the same time");
        }

        updatedReservation.Id = res.Id;
        updatedReservation.FarmingCompanyId = res.FarmingCompanyId;
        updatedReservation.WaterCompanyId = res.WaterCompanyId;
        updatedReservation.Timestamp = res.Timestamp;       // non sovrascrivere il timestamp
        
        _reservations.Entry(res).CurrentValues.SetValues(updatedReservation);
        await _context.SaveChangesAsync();

        return updatedReservation;
    }
    
    public async IAsyncEnumerable<Reservation> GetWaterComp(int waterCompanyId)
    {
        var waterCompany = await _waterCompanies.Get(waterCompanyId);
        foreach (var r in _reservations.Where(r => r.WaterCompanyId == waterCompany.Id)) yield return r;
    }

}