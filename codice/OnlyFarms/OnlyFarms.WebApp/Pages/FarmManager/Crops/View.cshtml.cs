using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlyFarms.WebApp.Pages.FarmManager.Crops;

[Authorize(Policy = Roles.FarmManager)]
public class View : BasePageModel
{
    public Crop Crop { get; set; }
    public IEnumerable<Actuator> Actuators { get; set; }
    public IEnumerable<Sensor> Sensors { get; set; }
    
    [BindProperty]
    public Actuator Actuator { get; set; }
    
    [BindProperty]
    public Sensor Sensor { get; set; }
    
    private readonly ICropComponentRepository<Actuator> _actuators;
    private readonly ICropComponentRepository<Sensor> _sensors;

    private readonly ICropComponentPropertyRepository<Command> _commands;
    private readonly ICropComponentPropertyRepository<Measurement> _measures;
    
    public View(ICropRepository crops, ICropComponentRepository<Actuator> actuators, ICropComponentRepository<Sensor> sensors, ICropComponentPropertyRepository<Command> commands, ICropComponentPropertyRepository<Measurement> measures)
        : base(crops)
    {
        _actuators = actuators;
        _sensors = sensors;

        _commands = commands;
        _measures = measures;
    }
    
    public async Task<IActionResult> OnGet(int farmingCompanyId, int cropId)
    {
        Crop = await _crops.Get(farmingCompanyId, cropId);
        Actuators = _actuators.GetAll(farmingCompanyId, cropId).ToBlockingEnumerable();
        Sensors = _sensors.GetAll(farmingCompanyId, cropId).ToBlockingEnumerable();

        return Page();
    }

    public async Task<IActionResult> OnPostDelete(int farmingCompanyId, int cropId)
    {
        await _crops.Delete(farmingCompanyId, cropId);
        return RedirectToPage("./Index", new {farmingCompanyId});
    }

    public async Task<IActionResult> OnPostActuator(int farmingCompanyId, int cropId)
    {
        await _actuators.Add(farmingCompanyId, cropId, Actuator);
        return RedirectToPage("./View", new { farmingCompanyId, cropId });
    }
    
    public async Task<IActionResult> OnPostSensor(int farmingCompanyId, int cropId)
    {
        await _sensors.Add(farmingCompanyId, cropId, Sensor);
        return RedirectToPage("./View", new { farmingCompanyId, cropId });
    }

    // TODO questi due metodi potrebbero essere un po' inefficienti, specie quando le tabelle cominciano a riempirsi di valori; sarebbe meglio farli eseguire direttamente al DB (es SELECT LAST(1) da tradurre in metodi EntityFramework)
    public string GetLastCommand(int actuatorId)
    {
        var last = _commands.GetAll(Crop.FarmingCompanyId, Crop.Id, actuatorId).ToBlockingEnumerable().LastOrDefault();
        return (last == null) ? "none" : last.State;
    }
    public string GetLastMeasurement(int sensorId)
    {
        var last = _measures.GetAll(Crop.FarmingCompanyId, Crop.Id, sensorId).ToBlockingEnumerable().LastOrDefault();
        return (last == null) ? "none" : last.Value + " " + last.MeasuringUnit;
    }
}