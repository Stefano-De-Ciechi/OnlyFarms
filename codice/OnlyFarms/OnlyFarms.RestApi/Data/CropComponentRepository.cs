using Microsoft.EntityFrameworkCore;
using OnlyFarms.Core.Data;
using OnlyFarms.Core.Models;

namespace OnlyFarms.RestApi.Data;

public class CropComponentRepository<T> : ICropComponentRepository<T> where T : class, ICropComponent
{
    private readonly DataContext _context;
    private readonly DbSet<T> _entities;

    public CropComponentRepository(DataContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        _context = context;
        _entities = _context.Set<T>();
    }

    public IAsyncEnumerable<T> GetAll(int farmingCompanyId, int cropId)
    {
        _CheckResourceExistence<FarmingCompany>(farmingCompanyId);
        _CheckResourceExistence<Crop>(cropId);
        
        return _entities.Where(c => c.CropId == cropId && c.FarmingCompanyId == farmingCompanyId).AsAsyncEnumerable();
    }
    
    public async Task<T> Get(int farmingCompanyId, int cropId, int componentId)
    {
        _CheckResourceExistence<FarmingCompany>(farmingCompanyId);
        _CheckResourceExistence<Crop>(cropId);

        var res = await _entities.FirstOrDefaultAsync(c => c.Id == componentId && c.CropId == cropId && c.FarmingCompanyId == farmingCompanyId);

        if (res == null)
        {
            throw new KeyNotFoundException($"no resource of type '{ typeof(T).Name }' with ID = { cropId }");
        }

        return res;
    }
    public async Task<T> Add(int farmingCompanyId, int cropId, T component)
    {
        _CheckResourceExistence<FarmingCompany>(farmingCompanyId);
        _CheckResourceExistence<Crop>(cropId);
        
        component.FarmingCompanyId = farmingCompanyId;
        component.CropId = cropId;

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

    private void _CheckResourceExistence<TR>(int id) where TR : class, IHasId
    {
        var resource = _context.Find<TR>(id);
        if (resource == null)
        {
            throw new KeyNotFoundException($"no resource of type '{ typeof(TR).Name }' with ID = { id }");
        }
    }
}