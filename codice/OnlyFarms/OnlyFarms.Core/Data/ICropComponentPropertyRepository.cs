namespace OnlyFarms.Core.Data;

public interface ICropComponentPropertyRepository<T> where T : class, IHasId, ICropComponentProperty
{
    public IAsyncEnumerable<T> GetAll(int farmingCompanyId, int cropId, int componentId);

    public IAsyncEnumerable<T> GetAll(int farmingCompanyId, int cropId, int componentId, DateTime? between, DateTime? and);
    public Task<T> Get(int farmingCompanyId, int cropId, int componentId, int id);
    public Task<T> Add(int farmingCompanyId, int cropId, int componentId, T property);
    public Task AddToAllComponents(int farmingCompanyId, int cropId, T property);
}