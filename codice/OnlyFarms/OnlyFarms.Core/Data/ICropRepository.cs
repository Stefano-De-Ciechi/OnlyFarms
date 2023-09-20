namespace OnlyFarms.Core.Data;

public interface ICropRepository
{
    IAsyncEnumerable<Crop> GetAll(int farmingCompanyId);
    Task<Crop> Get(int farmingCompanyId, int cropId);
    Task<Crop> Add(int farmingCompanyId, Crop crop);
    Task<Crop> Update(int farmingCompanyId, int cropId, Crop cropUpdate);
    Task<Crop> Delete(int farmingCompanyId, int cropId);
}