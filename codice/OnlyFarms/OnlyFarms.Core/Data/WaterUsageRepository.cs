namespace OnlyFarms.Core.Data;

public class WaterUsageRepository : IWaterUsageRepository
{
    private readonly DataContext _context;
    private readonly DbSet<WaterUsage> _usages;
    private readonly ICompanyRepository<FarmingCompany> _companies;
    private readonly ICropRepository _crops;

    public WaterUsageRepository(DataContext context, ICompanyRepository<FarmingCompany> companies, ICropRepository crops)
    {
        _context = context;
        _usages = _context.Set<WaterUsage>();
        _companies = companies;
        _crops = crops;
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

    public async IAsyncEnumerable<WaterUsage> GetAllByCrop(int farmingCompanyId, int cropId, DateTime? between = null, DateTime? and = null)
    {
        var company = await _companies.Get(farmingCompanyId);
        var crop = await _crops.Get(company.Id, cropId);

        if (between == null)
        {
            foreach (var w in _usages.Where(u => u.FarmingCompanyId == company.Id && u.CropId == crop.Id)) yield return w;
        }

        else
        {
            and ??= DateTime.Now;
            foreach (var w in _usages.Where(u => u.FarmingCompanyId == company.Id && u.CropId == crop.Id && u.Timestamp >= between && u.Timestamp <= and)) yield return w;
        }
    }

    public async Task<WaterUsage> Get(int id)
    {
        //var company = await _companies.Get(farmingCompanyId);
        var res = await _usages.FirstOrDefaultAsync(u => u.Id == id);

        if (res == null)
        {
            throw new NotFoundException<WaterUsage>(id);
        }

        return res;
    }

    public async Task<WaterUsage> Add(int farmingCompanyId, int cropId, WaterUsage usage)
    {
        var company = await _companies.Get(farmingCompanyId);
        var crop = await _crops.Get(farmingCompanyId, cropId);
        
        // controlla se e' gia' stato registrato un WaterUsage nella stessa data di quello che si sta tentando di aggiungere
        var last = await _usages.FirstOrDefaultAsync(u => u.FarmingCompanyId == company.Id && u.CropId == crop.Id && u.Timestamp.Date == usage.Timestamp.Date);
        
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
        usage.CropId = crop.Id;

        await _usages.AddAsync(usage);
        await _context.SaveChangesAsync();

        return usage;
    }

    public async Task<IEnumerable<(DateTime, int)>> GetTotalWaterUsage(int farmingCompanyId)
    {
        var company = await _companies.Get(farmingCompanyId);
        var usages = GetAll(company.Id);

        // prende la lista di waterUsages di ogni crop ed esegue la somma di utilizzi per giornata
        var result = usages.ToBlockingEnumerable()
            .GroupBy(u => u.Timestamp.Date)
            .Select(group => (group.Key, group.Sum(u => u.ConsumedQuantity)));

        return result;
    }

    public async Task<int> GetTotalWaterUsage(int farmingCompanyId, DateTime? day)
    {
        day ??= DateTime.Today;
        
        var company = await _companies.Get(farmingCompanyId);
        var usages = GetAll(company.Id);
        
        // prende la lista di waterUsage della FarmingCompany, filtra per il giorno passato come parametro e calcola la quantita' totale
        var result = usages.ToBlockingEnumerable()
            .Where(u => u.Timestamp.Date == day)
            .GroupBy(u => u.Timestamp.Date)
            .Select(group => group.Sum(u => u.ConsumedQuantity))
            .FirstOrDefault();

        return result;
    }
}