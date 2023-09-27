namespace OnlyFarms.Core.Data;

public interface IWaterLimitRepository
{
    IAsyncEnumerable<WaterLimit> GetAll(int waterCompanyId);
    Task<WaterLimit> Get(int id);
    Task<WaterLimit> Get(int waterCompanyId, int farmingCompanyId);
    Task<WaterLimit> Add(int waterCompanyId, int farmingCompanyId, int limit);
    Task<WaterLimit> Update(int id, int newLimit);
    Task<WaterLimit> Delete(int id);
}