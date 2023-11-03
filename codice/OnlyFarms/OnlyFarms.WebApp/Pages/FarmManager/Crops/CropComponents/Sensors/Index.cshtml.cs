using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlyFarms.WebApp.Pages.FarmManager.Crops.CropComponents.Sensors;

[Authorize(Policy = Roles.FarmManager)]
public class Index : BasePageModel
{
    private readonly ICropComponentRepository<Sensor> _sensors;
    private readonly ICropComponentPropertyRepository<Measurement> _measurements;

    public Sensor Sensor { get; set; }
    public IEnumerable<Measurement> Measurements { get; set; }
    
    public Index(ICropRepository crops, ICropComponentRepository<Sensor> sensors, ICropComponentPropertyRepository<Measurement> measurements)
        : base(crops)
    {
        _sensors = sensors;
        _measurements = measurements;
    }
    
    public async Task<IActionResult> OnGet(int farmingCompanyId, int cropId, int sensorId)
    {
        Sensor = await _sensors.Get(farmingCompanyId, cropId, sensorId);
        Measurements = _measurements.GetAll(farmingCompanyId, cropId, sensorId).ToBlockingEnumerable();
        
        return Page();
    }
    
    public async Task<IActionResult> OnPostDelete(int farmingCompanyId, int cropId, int sensorId)
    {
        await _sensors.Delete(farmingCompanyId, cropId, sensorId);
        return RedirectToPage("/FarmManager/Crops/View", new {farmingCompanyId, cropId});
    }
}