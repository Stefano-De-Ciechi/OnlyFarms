using Microsoft.EntityFrameworkCore;
using OnlyFarms.Core.Data;
using OnlyFarms.Core.Models;

namespace OnlyFarms.RestApi.Data;

// C e' il tipo del componente (Attuatore o Sensore), CP e' il tipo della proprieta' del componente (ActuatorCommand o Measurement)
public class CropComponentPropertyRepository<C, CP> : ICropComponentPropertyRepository<CP> where C : class, IHasId, ICropComponent where CP : class, IHasId, ICropComponentProperty
{
    private readonly DataContext _context;
    private readonly DbSet<CP> _entities;
    private readonly ICropComponentRepository<C> _components;

    public CropComponentPropertyRepository(DataContext context, ICropComponentRepository<C> components)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(components);
        
        _context = context;
        _entities = _context.Set<CP>();
        _components = components;
    }
    
    /*public async IAsyncEnumerable<CP> GetAll(int farmingCompanyId, int cropId, int componentId)
    {
        var component = await _components.Get(farmingCompanyId, cropId, componentId);
        foreach (var cp in _entities.Where(cp => cp.ComponentId == component.Id)) yield return cp;
    }*/
    
    public IAsyncEnumerable<CP> GetAll(int farmingCompanyId, int cropId, int componentId)
    {
        _CheckResourceExistence<FarmingCompany>(farmingCompanyId);
        _CheckResourceExistence<Crop>(cropId);
        _CheckResourceExistence<Actuator>(componentId);

        return _entities.Where(cp => cp.ComponentId == componentId).AsAsyncEnumerable();
    }

    public async Task<CP> Get(int farmingCompanyId, int cropId, int componentId, int id)
    {
        var component = await _components.Get(farmingCompanyId, cropId, componentId);
        var res = await _entities.FirstOrDefaultAsync(cp => cp.ComponentId == component.Id && cp.Id == id);

        if (res == null)
        {
            throw new KeyNotFoundException($"no resource of type {typeof(CP).Name} with ID = {id}");
        }

        return res;
    }

    public async Task<CP> Add(int farmingCompanyId, int cropId, int componentId, CP property)
    {
        var component = await _components.Get(farmingCompanyId, cropId, componentId);

        property.ComponentId = component.Id;

        await _entities.AddAsync(property);
        await _context.SaveChangesAsync();

        return property;
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