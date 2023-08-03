namespace OnlyFarms.Core.Data;

public interface IRepository<T> where T : class, IHasId
{
    Task<T?> Add(T item);
    Task<T?> Delete(int id);
    Task<T?> Update(int id, T updatedItem);
    Task<T?> Get(int id);
    IAsyncEnumerable<T> GetAll();
}