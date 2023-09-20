namespace OnlyFarms.Core.Data;

public class CropComponentRepository<T> : ICropComponentRepository<T> where T : class, ICropComponent
{
    private readonly DataContext _context;
    private readonly DbSet<T> _entities;
    private readonly ICropRepository _crops;

    public CropComponentRepository(DataContext context, ICropRepository crops)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(crops);
        
        _context = context;
        _entities = _context.Set<T>();
        _crops = crops;
    }

    public async IAsyncEnumerable<T> GetAll(int farmingCompanyId, int cropId)
    {
        var crop = await _crops.Get(farmingCompanyId, cropId);
        foreach (var c in _entities.Where(c => c.CropId == crop.Id && c.FarmingCompanyId == crop.FarmingCompanyId)) yield return c;
    }
    
    public async Task<T> Get(int farmingCompanyId, int cropId, int componentId)
    {
        var crop = await _crops.Get(farmingCompanyId, cropId);
        var res = await _entities.FirstOrDefaultAsync(c => c.Id == componentId && c.CropId == crop.Id && c.FarmingCompanyId == crop.FarmingCompanyId);

        if (res == null)
        {
            throw new NotFoundException<T>(componentId);
        }

        return res;
    }
    public async Task<T> Add(int farmingCompanyId, int cropId, T component)
    {
        var crop = await _crops.Get(farmingCompanyId, cropId);
        
        component.FarmingCompanyId = crop.FarmingCompanyId;
        component.CropId = crop.Id;

        await _entities.AddAsync(component);
        await _context.SaveChangesAsync();

        return component;
    }

    public async Task<T> Delete(int farmingCompanyId, int cropId, int componentId)
    {
        var component = await Get(farmingCompanyId, cropId, componentId);
        
        _entities.Remove(component);
        await _context.SaveChangesAsync();

        return component;
    }
}