namespace OnlyFarms.RestApi.Data;

public class WaterUsageRepository : IWaterUsageRepository
{
    private readonly DataContext _context;
    private readonly DbSet<WaterUsage> _usages;
    private readonly IRepository<FarmingCompany> _companies;

    public WaterUsageRepository(DataContext context, IRepository<FarmingCompany> companies)
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

        usage.FarmingCompanyId = company.Id;

        await _usages.AddAsync(usage);
        await _context.SaveChangesAsync();

        return usage;
    }
}