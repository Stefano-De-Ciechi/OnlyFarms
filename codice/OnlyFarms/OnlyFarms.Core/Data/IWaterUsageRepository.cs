namespace OnlyFarms.Core.Data;

public interface IWaterUsageRepository
{
    IAsyncEnumerable<WaterUsage> GetAll(int farmingCompanyId);
    IAsyncEnumerable<WaterUsage> GetAll(int farmingCompanyId, DateTime? between, DateTime? and);
    IAsyncEnumerable<WaterUsage> GetAllByCrop(int farmingCompanyId, int cropId, DateTime? between, DateTime? and);
    Task<WaterUsage> Get(int id);
    Task<WaterUsage> Add(int farmingCompanyId, int cropId, WaterUsage usage);

    Task<IEnumerable<(DateTime, int)>> GetTotalWaterUsage(int farmingCompanyId);
    Task<int> GetTotalWaterUsage(int farmingCompanyId, DateTime? day);
}