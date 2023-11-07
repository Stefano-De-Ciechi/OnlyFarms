namespace OnlyFarms.Core.Data;

public class WaterLimitRepository : IWaterLimitRepository
{
    private readonly DataContext _context;
    private readonly DbSet<WaterLimit> _limits;
    private readonly ICompanyRepository<FarmingCompany> _farmingCompanies;
    private readonly ICompanyRepository<WaterCompany> _waterCompanies;

    public WaterLimitRepository(DataContext context, ICompanyRepository<FarmingCompany> farmingCompanies, ICompanyRepository<WaterCompany> waterCompanies)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(farmingCompanies);
        ArgumentNullException.ThrowIfNull(waterCompanies);

        _context = context;
        _limits = _context.Set<WaterLimit>();
        _farmingCompanies = farmingCompanies;
        _waterCompanies = waterCompanies;
    }
    
    public async IAsyncEnumerable<WaterLimit> GetAll(int waterCompanyId)
    {
        // TODO quando si esegue la query, il campo ID di tutte le farmingCompany viene restituito come nullo! --> bisogna usare il medoto di EF .Load(), vedi https://ef.readthedocs.io/en/staging/querying/related-data.html 
        var waterCompany = await _waterCompanies.Get(waterCompanyId);
        foreach (var wl in _limits.Where(l => l.WaterCompany.Id == waterCompany.Id)) yield return wl;
    }

    public async Task<WaterLimit> Get(int id)
    {
        var limit = await _limits.FindAsync(id);
        if (limit == null)
        {
            throw new NotFoundException<WaterLimit>(id);
        }

        return limit;
    }

    public async Task<WaterLimit> Get(int waterCompanyId, int farmingCompanyId)
    {
        var farmingCompany = await _farmingCompanies.Get(farmingCompanyId);
        var waterCompany = await _waterCompanies.Get(waterCompanyId);

        var res = await _limits.FirstOrDefaultAsync(l => l.FarmingCompany.Id == farmingCompany.Id && l.WaterCompany.Id == waterCompany.Id);
        if (res == null)
        {
            throw new NotFoundException<WaterLimit>();
        }

        return res;
    }

    public async Task<WaterLimit> Add(int waterCompanyId, int farmingCompanyId, int limit)
    {
        var farmingCompany = await _farmingCompanies.Get(farmingCompanyId);
        var waterCompany = await _waterCompanies.Get(waterCompanyId);

        try
        {
            var previous = await Get(waterCompanyId, farmingCompanyId);
            throw new DbUpdateException("can't have more than one active water limit between the same companies at the same time");
        }
        catch (NotFoundException<WaterLimit> e)
        {
            // se si riceve un'eccezione NotFound vuol dire che non esistono limit tra le due aziende, quindi si puo' eseguire l'aggiunta al DB
        }

        var newLimit = new WaterLimit()
        {
            FarmingCompanyId = farmingCompany.Id,
            FarmingCompany = farmingCompany,
            WaterCompanyId = waterCompany.Id,
            WaterCompany = waterCompany,
            Limit = limit
        };

        await _limits.AddAsync(newLimit);
        await _context.SaveChangesAsync();

        return newLimit;
    }

    public async Task<WaterLimit> Update(int id, int newLimit)
    {
        // TODO eseguire dei controlli sul nuovo limite? cosa fare se il limite viene superato?
        var limit = await Get(id);
        limit.Limit = newLimit;
        
        _limits.Entry(limit).CurrentValues.SetValues(limit);
        await _context.SaveChangesAsync();

        return limit;
    }

    public async Task<WaterLimit> Delete(int id)
    {
        var limit = await Get(id);

        _limits.Remove(limit);
        await _context.SaveChangesAsync();

        return limit;
    }
}