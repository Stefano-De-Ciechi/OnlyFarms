namespace OnlyFarms.Core.Data;

public interface ICropComponentRepository<T> where T : class, IHasId, ICropComponent
{
    Task<T?> Add(int farmingCompanyId, int cropId, T component);
    Task<T?> Delete(int farmingCompanyId, int cropId, int componentId);
    Task<T?> Get(int farmingCompanyId, int cropId, int componentId);
    IAsyncEnumerable<T> GetAll(int farmingCompanyId, int cropId);
}