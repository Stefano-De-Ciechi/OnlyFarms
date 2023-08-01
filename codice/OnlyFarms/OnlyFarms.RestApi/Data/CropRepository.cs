using Microsoft.EntityFrameworkCore;
using OnlyFarms.Core.Data;
using OnlyFarms.Core.Models;

namespace OnlyFarms.RestApi.Data;

// TODO la classe potrebbe essere resa generica per funzionare con tutte le entita' che richiedono un farmingCompanyId (ma devono anche implementare tutti i verbi HTTP)
public class CropRepository : ICropRepository
{
    private readonly DataContext _context;
    
    public CropRepository(DataContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        _context = context;
    }
    
    public async Task<Crop?> Add(int farmingCompanyId, Crop crop)
    {
        var company = await _context.FarmingCompanies.FirstOrDefaultAsync(f => f.Id == farmingCompanyId);
        if (company == null)
        {
            return null;
        }

        crop.FarmingCompanyId = farmingCompanyId;
        company.WaterSupply += crop.WaterNeeds;     // TODO rimuovere se questo non e' il comportamento voluto

        await _context.Crops.AddAsync(crop);
        await _context.SaveChangesAsync();

        return crop;
    }

    public async Task<Crop?> Update(int farmingCompanyId, int cropId, Crop cropUpdate)
    {
        var crop = await Get(farmingCompanyId, cropId);
        if (crop == null)
        {
            return null;
        }

        cropUpdate.Id = crop.Id;
        cropUpdate.FarmingCompanyId = crop.FarmingCompanyId;

        /*
         * spiegazione: se viene aggiornata la quantita' d'acqua necessaria alla coltivazione, allora il totale di acqua necessaria all'azienda deve cambiare di conseguenza
        */
        if (!crop.WaterNeeds.Equals(cropUpdate.WaterNeeds))
        {
            var company = await _context.FarmingCompanies.FindAsync(farmingCompanyId);
            company!.WaterSupply -= crop.WaterNeeds;    // TODO rimuovere se questo non e' il comportamento voluto
            company!.WaterSupply += cropUpdate.WaterNeeds;
        }
        
        _context.Crops.Entry(crop).CurrentValues.SetValues(cropUpdate);
        await _context.SaveChangesAsync();

        return cropUpdate;
    }

    public async Task<Crop?> Delete(int farmingCompanyId, int cropId)
    {
        var crop = await Get(farmingCompanyId, cropId);
        if (crop == null)
        {
            return null;
        }

        _context.Remove(crop);      // TODO verificare che vengano eliminati dal DB anche tutte le entita' legate alla coltivazione (es. sensori, attuatori, ...)
        var company = await _context.FarmingCompanies.FindAsync(farmingCompanyId);
        company!.WaterSupply -= crop.WaterNeeds;    // TODO rimuovere se questo non e' il comportamento voluto
        await _context.SaveChangesAsync();

        return crop;
    }

    public Task<Crop?> Get(int farmingCompanyId, int cropId) => _context.Crops.FirstOrDefaultAsync(c => c.Id == cropId && c.FarmingCompanyId == farmingCompanyId);

    public IAsyncEnumerable<Crop> GetAll(int farmingCompanyId) => _context.Crops.Where(c => c.FarmingCompanyId == farmingCompanyId).AsAsyncEnumerable();
    
}