using OnlyFarms.Core.Models;

namespace OnlyFarms.Core.Data;

public interface IWaterUsageRepository
{
    IAsyncEnumerable<WaterUsage> GetAll(int farmingCompanyId);
    Task<WaterUsage> Get(int farmingCompanyId, int id);
    Task<WaterUsage> Add(int farmingCompanyId, WaterUsage usage);
}