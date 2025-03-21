namespace OnlyFarms.Core.Data;

public class CropRepository : ICropRepository
{
    private readonly DataContext _context;
    private readonly DbSet<Crop> _crops;
    private readonly ICompanyRepository<FarmingCompany> _companies;

    public CropRepository(DataContext context, ICompanyRepository<FarmingCompany> companies)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(companies);

        _context = context;
        _crops = _context.Set<Crop>();
        _companies = companies;
    }

    public async IAsyncEnumerable<Crop> GetAll(int farmingCompanyId)
    {
        var company = await _companies.Get(farmingCompanyId);
        foreach (var c in _crops.Where(c => c.FarmingCompanyId == company.Id)) yield return c;
    }

    public async Task<Crop> Get(int farmingCompanyId, int cropId)
    {
        var company = await _companies.Get(farmingCompanyId);
        var res = await _crops.FirstOrDefaultAsync(c => c.Id == cropId && c.FarmingCompanyId == company.Id);
        if (res == null)
        {
            throw new NotFoundException<Crop>(cropId);
        }

        return res;
    }
    
    public async Task<Crop> Add(int farmingCompanyId, Crop crop)
    {
        var company = await _companies.Get(farmingCompanyId);

        crop.FarmingCompanyId = company.Id;
        company.WaterSupply += crop.WaterNeeds;     // il campo WaterSupply viene usato come quantita' totale di acqua necessaria per tutte le coltivazioni dell'azienda

        await _crops.AddAsync(crop);
        await _context.SaveChangesAsync();

        return crop;
    }

    public async Task<Crop> Update(int farmingCompanyId, int cropId, Crop cropUpdate)
    {
        var crop = await Get(farmingCompanyId, cropId);
        
        cropUpdate.Id = crop.Id;
        cropUpdate.FarmingCompanyId = crop.FarmingCompanyId;
        
        // spiegazione: se viene aggiornata la quantita' d'acqua necessaria alla coltivazione, allora il totale di acqua necessaria all'azienda deve cambiare di conseguenza
        if (!crop.WaterNeeds.Equals(cropUpdate.WaterNeeds))
        {
            var company = await _companies.Get(farmingCompanyId);
            company.WaterSupply -= crop.WaterNeeds;
            company.WaterSupply += cropUpdate.WaterNeeds;
        }
        
        _crops.Entry(crop).CurrentValues.SetValues(cropUpdate);
        await _context.SaveChangesAsync();

        return cropUpdate;
    }

    public async Task<Crop> Delete(int farmingCompanyId, int cropId)
    {
        var crop = await Get(farmingCompanyId, cropId);
        
        _crops.Remove(crop);
        
        var company = await _companies.Get(farmingCompanyId);
        company.WaterSupply -= crop.WaterNeeds;
        
        await _context.SaveChangesAsync();

        return crop;
    }
}