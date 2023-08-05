namespace OnlyFarms.RestApi.Data;

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

    public IAsyncEnumerable<T> GetAll() => _entities.AsAsyncEnumerable();
    
    public async Task<T> Get(int id)
    {   
        var res = await _entities.FindAsync(id);
        if (res == null)
        {
            throw new NotFoundException<T>(id);
        }

        return res;
    }
    
    public async Task<T> Add(T entity)     // TODO questo metodo dovrebbe ritornare l'ID dell'entità inserita
    {
        await _entities.AddAsync(entity);
        await _dataContext.SaveChangesAsync();
        
        return entity;
    }

    public async Task<T> Update(int id, T updatedItem)     // TODO testare che questa funzione di modifica funzioni correttamente!
    {
        var entity = await Get(id);
        
        updatedItem.Id = entity.Id;     // aggiunto per non ricevere un errore del tipo "the property is part of a key and so cannot be modified or marked as modified ..."
        
        _entities.Entry(entity).CurrentValues.SetValues(updatedItem);
        await _dataContext.SaveChangesAsync();
        
        return updatedItem;
    }

    public async Task<T> Delete(int id)    // TODO questo metodo dovrebbe ritornare l'entità eliminata
    {
        var entity = await Get(id);
        
        _dataContext.Remove(entity);        // TODO verificare che vengano rimosse anche tutte le entita' collegate (es. se si cancella un azienda agricola devono essere eliminate anche tutte le sue coltivazioni, sensori, attuatori, prenotazioni, ...
        await _dataContext.SaveChangesAsync();
        
        return entity;
    }
}
