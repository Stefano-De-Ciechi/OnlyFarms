using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace OnlyFarms.Data;

/* classe da usare come repository solo all'interno del server che implementa la REST API */

public class DataContextRepository<T> : IRepository<T> where T : class, IHasId
{
    readonly DbContext _dataContext;

    readonly DbSet<T> _entities;

    public DataContextRepository(DbContext dataContext)
    {
        ArgumentNullException.ThrowIfNull(dataContext);

        _dataContext = dataContext;
        _entities = _dataContext.Set<T>();
    }

    public async Task Add(T entity)
    {
        await _entities.AddAsync(entity);
        await _dataContext.SaveChangesAsync();
    }

    public async Task Update(int id, T updatedItem)     // TODO testare che questa funzione di modifica funzioni correttamente!
    {
        var entity = await Get(id);
        if (entity == null)
        {
            return;
        }

        _entities.Entry(entity).CurrentValues.SetValues(updatedItem);
        await _dataContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var entity = await Get(id);
        if (entity != null)
        {
            _dataContext.Remove(entity);
            await _dataContext.SaveChangesAsync();
        }
    }

    public async Task<T?> Get(int id)
    {
        return await _dataContext.FindAsync<T>(id);
    }

    public IAsyncEnumerable<T> GetAll()
    {
        return _entities.AsAsyncEnumerable();
    }

}
