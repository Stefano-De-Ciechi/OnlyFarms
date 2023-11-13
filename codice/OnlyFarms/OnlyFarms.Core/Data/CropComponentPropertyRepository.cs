using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OnlyFarms.Core.Data;

// C e' il tipo del componente (Attuatore o Sensore), CP e' il tipo della proprieta' del componente (Command o Measurement)
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
    
    public async IAsyncEnumerable<CP> GetAll(int farmingCompanyId, int cropId, int componentId)
    {
        var component = await _components.Get(farmingCompanyId, cropId, componentId);
        foreach (var c in _entities.Where(c => c.ComponentId == component.Id)) yield return c;
    }

    public async IAsyncEnumerable<CP> GetAll(int farmingCompanyId, int cropId, int componentId, DateTime? between, DateTime? and)
    {
        and ??= DateTime.Now;

        var component = await _components.Get(farmingCompanyId, cropId, componentId);
        foreach (var c in _entities.Where(c => c.ComponentId == component.Id && c.Timestamp >= between && c.Timestamp <= and)) yield return c;
    }
    
    public async Task<CP> Get(int farmingCompanyId, int cropId, int componentId, int id)
    {
        var component = await _components.Get(farmingCompanyId, cropId, componentId);
        var res = await _entities.FirstOrDefaultAsync(cp => cp.ComponentId == component.Id && cp.Id == id);

        if (res == null)
        {
            throw new NotFoundException<CP>(id);
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

    public async Task AddToAllComponents(int farmingCompanyId, int cropId, CP property)
    {
        // metodo valido solo per oggetti di tipo Command degli Actuator
        if (typeof(C) == typeof(Measurement))
        {
            throw new UnsupportedContentTypeException("this endpoint is not supported for Measurement objects");
        }
        
        // metodo per assegnare a tutti gli attuatori di una crop lo stesso comando
        var actuators = _components.GetAll(farmingCompanyId, cropId);

        var tmp = (property as Command)!;
        foreach (var actuator in actuators.ToBlockingEnumerable())
        {
            /* spiegazione: serve creare un nuovo comando con la copia dei valori ricevuti dal body della request, perche' ad ogni add
               i valori tipo Id, ComponentId ecc. vengono automaticamente fillati da EntityFramework, e se si prova a ri-usare lo stesso
               oggetto (che ha il valore Id uguale al precedente) si ottiene una Eccezione SQL sulla violazione del constraint sulla
               unicita' del valore di Id 
            */
            var command = new Command()
            {
                State = tmp.State,
                Timestamp = tmp.Timestamp
            };
            await Add(actuator.FarmingCompanyId, actuator.CropId, actuator.Id, (command as CP)!);
        }
    }
    
}