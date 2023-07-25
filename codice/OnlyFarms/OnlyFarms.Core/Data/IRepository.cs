namespace OnlyFarms.Data;

public interface IRepository<T> where T : class, IHasId
{
    // TODO verificare se e' necessario che i risultati ritornati siano nullable o meno (T o T?)
    Task<T?> Add(T item);
    Task<T?> Delete(int id);
    Task<T?> Update(int id, T updatedItem);
    Task<T?> Get(int id);
    IAsyncEnumerable<T> GetAll();
}