namespace OnlyFarms.Core.Data;

public interface IRepository<T> where T : class, IHasId
{
    IAsyncEnumerable<T> GetAll();
    Task<T> Get(int id);
    Task<T> Add(T item);
    Task<T> Delete(int id);
    Task<T> Update(int id, T updatedItem);
}