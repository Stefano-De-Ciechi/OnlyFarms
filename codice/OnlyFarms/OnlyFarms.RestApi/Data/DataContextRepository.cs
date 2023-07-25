using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace OnlyFarms.Data;

/* classe da usare come repository solo all'interno del server che implementa la REST API */

public class DataContextRepository<T> : IRepository<T> where T : class, IHasId
{
    private readonly DbContext _dataContext;

    private readonly DbSet<T> _entities;

    public DataContextRepository(DbContext dataContext)
    {
        ArgumentNullException.ThrowIfNull(dataContext);

        _dataContext = dataContext;
        _entities = _dataContext.Set<T>();
    }

    public async Task<T?> Add(T entity)     // TODO questo metodo dovrebbe ritornare l'ID dell'entità inserita
    {
        await _entities.AddAsync(entity);
        await _dataContext.SaveChangesAsync();
        
        return entity;
    }

    public async Task<T?> Update(int id, T updatedItem)     // TODO testare che questa funzione di modifica funzioni correttamente!
    {
        var entity = await Get(id);
        if (entity == null)
        {
            return null;
        }

        _entities.Entry(entity).CurrentValues.SetValues(updatedItem);
        await _dataContext.SaveChangesAsync();
        
        return updatedItem;
    }

    public async Task<T?> Delete(int id)    // TODO questo metodo dovrebbe ritornare l'entità eliminata
    {
        var entity = await Get(id);
        
        if (entity == null) return null;
        
        _dataContext.Remove(entity);
        await _dataContext.SaveChangesAsync();
        
        return entity;
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
