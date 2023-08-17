namespace OnlyFarms.RestApi.Data;

public class CompanyRepository<T> : ICompanyRepository<T> where T : class, ICompany
{
    private readonly DbContext _dataContext;
    private readonly DbSet<T> _entities;

    public CompanyRepository(DbContext dataContext)
    {
        ArgumentNullException.ThrowIfNull(dataContext);

        _dataContext = dataContext;
        _entities = _dataContext.Set<T>();
    }

    public IAsyncEnumerable<T> GetAll() => _entities.AsAsyncEnumerable();

    public IAsyncEnumerable<T> GetAll(string city) => _entities.Where(c => string.Equals(c.City.ToLower(), city.ToLower())).AsAsyncEnumerable();
    
    public async Task<T> Get(int id)
    {   
        var res = await _entities.FindAsync(id);
        if (res == null)
        {
            throw new NotFoundException<T>(id);
        }

        return res;
    }
    
    public async Task<T> Add(T entity)
    {
        await _entities.AddAsync(entity);
        await _dataContext.SaveChangesAsync();
        
        return entity;
    }

    public async Task<T> Update(int id, T updatedItem)
    {
        var entity = await Get(id);
        
        updatedItem.Id = entity.Id;     // aggiunto per non ricevere un errore del tipo "the property is part of a key and so cannot be modified or marked as modified ..."
        
        _entities.Entry(entity).CurrentValues.SetValues(updatedItem);
        await _dataContext.SaveChangesAsync();
        
        return updatedItem;
    }

    public async Task<T> Delete(int id)
    {
        var entity = await Get(id);
        
        _dataContext.Remove(entity);
        await _dataContext.SaveChangesAsync();
        
        return entity;
    }
}
