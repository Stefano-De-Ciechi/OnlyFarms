namespace OnlyFarms.Core.Data;

public interface ICompanyRepository<T> where T : class, ICompany
{
    IAsyncEnumerable<T> GetAll();
    IAsyncEnumerable<T> GetAll(string city);
    Task<T> Get(int id);
    Task<T> Add(T item);
    Task<T> Delete(int id);
    Task<T> Update(int id, T updatedItem);
}