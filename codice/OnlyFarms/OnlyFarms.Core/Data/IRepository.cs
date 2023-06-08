namespace OnlyFarms.Data;

public interface IRepository<T> where T : class, IHasId
{
    Task Add(T item);
    Task Delete(int id);
    Task Update(int id, T updatedItem);
    Task<T?> Get(int id);
    IAsyncEnumerable<T> GetAll();
}