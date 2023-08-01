using OnlyFarms.Core.Models;

namespace OnlyFarms.Core.Data;

public interface ICropRepository
{
    Task<Crop?> Add(int farmingCompanyId, Crop crop);
    Task<Crop?> Update(int farmingCompanyId, int cropId, Crop cropUpdate);
    Task<Crop?> Delete(int farmingCompanyId, int cropId);
    Task<Crop?> Get(int farmingCompanyId, int cropId);
    IAsyncEnumerable<Crop> GetAll(int farmingCompanyId);
}