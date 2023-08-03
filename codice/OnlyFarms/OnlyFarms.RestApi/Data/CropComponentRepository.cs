using Microsoft.EntityFrameworkCore;
using OnlyFarms.Core.Data;
using OnlyFarms.Core.Models;

namespace OnlyFarms.RestApi.Data;

public class CropComponentRepository<T> : ICropComponentRepository<T> where T : class, ICropComponent
{
    private readonly DataContext _context;
    private readonly DbSet<T> _entities;

    public CropComponentRepository(DataContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        _context = context;
        _entities = _context.Set<T>();
    }

    public async Task<T?> Add(int farmingCompanyId, int cropId, T component)
    {
        var company = await _context.FarmingCompanies.FindAsync(farmingCompanyId);
        var crop = await _context.Crops.FindAsync(cropId);

        if (company == null)
        {
            throw new KeyNotFoundException($"no resource of type '{ nameof(FarmingCompany) }' with ID = { farmingCompanyId }");   // TODO pensare ad un meccanismo di gestione delle eccezioni se si passa un id non valido di una azienda agricola o di una coltivazione
        }

        if (crop == null)
        {
            throw new KeyNotFoundException($"no resource of type '{ nameof(Crop) }' with ID = { cropId }");
        }
        
        component.FarmingCompanyId = farmingCompanyId;
        component.CropId = cropId;

        await _entities.AddAsync(component);
        await _context.SaveChangesAsync();

        return component;
    }

    public async Task<T?> Delete(int farmingCompanyId, int cropId, int componentId)
    {
        var component = await Get(farmingCompanyId, cropId, componentId);

        if (component == null)
        {
            return null;
        }

        _entities.Remove(component);
        await _context.SaveChangesAsync();

        return component;
    }

    public async Task<T?> Get(int farmingCompanyId, int cropId, int componentId) => await _entities.FirstOrDefaultAsync(c => c.Id == componentId && c.CropId == cropId && c.FarmingCompanyId == farmingCompanyId);

    public IAsyncEnumerable<T> GetAll(int farmingCompanyId, int cropId) => _entities.Where(c => c.CropId == cropId && c.FarmingCompanyId == farmingCompanyId).AsAsyncEnumerable();
}