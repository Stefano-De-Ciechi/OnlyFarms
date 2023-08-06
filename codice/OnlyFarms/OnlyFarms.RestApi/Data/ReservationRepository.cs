namespace OnlyFarms.RestApi.Data;

public class ReservationRepository : IReservationRepository
{
    private readonly DataContext _context;
    private readonly DbSet<Reservation> _reservations;
    private readonly IRepository<FarmingCompany> _farmingCompanies;
    private readonly IRepository<WaterCompany> _waterCompanies;

    public ReservationRepository(DataContext context, IRepository<FarmingCompany> farmingCompanies, IRepository<WaterCompany> waterCompanies)
    {
        _context = context;
        _reservations = _context.Set<Reservation>();
        _farmingCompanies = farmingCompanies;
        _waterCompanies = waterCompanies;
    }

    public async IAsyncEnumerable<Reservation> GetAll(int farmingCompanyId, int waterCompanyId)
    {
        var farmingCompany = await _farmingCompanies.Get(farmingCompanyId);
        var waterCompany = await _waterCompanies.Get(waterCompanyId);
        foreach (var r in _reservations.Where(r => r.FarmingCompanyId == farmingCompany.Id && r.WaterCompanyId == waterCompany.Id)) yield return r;
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
        updatedReservation.TimeStamp = res.TimeStamp;       // non sovrascrivere il timestamp
        
        _reservations.Entry(res).CurrentValues.SetValues(updatedReservation);
        await _context.SaveChangesAsync();

        return updatedReservation;
    }
}