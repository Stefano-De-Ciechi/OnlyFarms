namespace OnlyFarms.Core.Data;

public class WaterUsageRepository : IWaterUsageRepository
{
    private readonly DataContext _context;
    private readonly DbSet<WaterUsage> _usages;
    private readonly ICompanyRepository<FarmingCompany> _companies;

    public WaterUsageRepository(DataContext context, ICompanyRepository<FarmingCompany> companies)
    {
        _context = context;
        _usages = _context.Set<WaterUsage>();
        _companies = companies;
    }

    public async IAsyncEnumerable<WaterUsage> GetAll(int farmingCompanyId)
    {
        var company = await _companies.Get(farmingCompanyId);
        foreach (var w in _usages.Where(u => u.FarmingCompanyId == company.Id)) yield return w;
    }

    public async IAsyncEnumerable<WaterUsage> GetAll(int farmingCompanyId, DateTime? between, DateTime? and)
    {
        and ??= DateTime.Now;
        
        var company = await _companies.Get(farmingCompanyId);
        foreach (var w in _usages.Where(u => u.FarmingCompanyId == company.Id && u.Timestamp >= between && u.Timestamp <= and)) yield return w;
    }

    public async Task<WaterUsage> Get(int farmingCompanyId, int id)
    {
        var company = await _companies.Get(farmingCompanyId);
        var res = await _usages.FirstOrDefaultAsync(u => u.Id == id && u.FarmingCompanyId == company.Id);

        if (res == null)
        {
            throw new NotFoundException<WaterUsage>(id);
        }

        return res;
    }

    public async Task<WaterUsage> Add(int farmingCompanyId, WaterUsage usage)
    {
        var company = await _companies.Get(farmingCompanyId);
        
        // controlla se e' gia' stato registrato un WaterUsage nella stessa data di quello che si sta tentando di aggiungere
        var last = await _usages.FirstOrDefaultAsync(u => u.FarmingCompanyId == company.Id && u.Timestamp.Date == usage.Timestamp.Date);
        
        if (last != null)
        {
            // se le date (considerando solo giorno/mese/anno) coincidono allora un WaterUsage e' gia' stato registrato oggi
            last.Timestamp = usage.Timestamp;
            last.ConsumedQuantity = usage.ConsumedQuantity;
            
            // si sovrascrivono i valori del WaterUsage gia' registrato (cambiano solo TimeStamp e ConsumedQuantity
            _usages.Entry(last).CurrentValues.SetValues(last);
            await _context.SaveChangesAsync();
            //throw new DbUpdateException("can't register two usages for the same company the same day; overwriting the previous value");
            return last;
        }
        
        usage.FarmingCompanyId = company.Id;

        await _usages.AddAsync(usage);
        await _context.SaveChangesAsync();

        return usage;
    }
}