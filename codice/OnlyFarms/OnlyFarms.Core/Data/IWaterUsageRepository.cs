namespace OnlyFarms.Core.Data;

public interface IWaterUsageRepository
{
    IAsyncEnumerable<WaterUsage> GetAll(int farmingCompanyId);
    IAsyncEnumerable<WaterUsage> GetAll(int farmingCompanyId, DateTime? between, DateTime? and);
    Task<WaterUsage> Get(int farmingCompanyId, int id);
    Task<WaterUsage> Add(int farmingCompanyId, WaterUsage usage);
}