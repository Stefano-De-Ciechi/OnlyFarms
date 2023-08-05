namespace OnlyFarms.Core.Data;

public interface ICropComponentPropertyRepository<T> where T : class, IHasId, ICropComponentProperty
{
    public IAsyncEnumerable<T> GetAll(int farmingCompanyId, int cropId, int componentId);
    public Task<T> Get(int farmingCompanyId, int cropId, int componentId, int id);
    public Task<T> Add(int farmingCompanyId, int cropId, int componentId, T property);
}