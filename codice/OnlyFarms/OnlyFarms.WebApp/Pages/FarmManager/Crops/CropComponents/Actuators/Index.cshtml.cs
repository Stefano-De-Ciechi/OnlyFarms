using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlyFarms.WebApp.Pages.FarmManager.Crops.CropComponents.Actuators;

// TODO Index di Attuatori e Index di Sensori potrebbero essere rese un'unica classe generica

[Authorize(Policy = Roles.FarmManager)]
public class Index : BasePageModel
{
    private readonly ICropComponentRepository<Actuator> _actuators;
    private readonly ICropComponentPropertyRepository<Command> _commands;
    
    public Actuator Actuator { get; set; }
    public IEnumerable<Command> Commands { get; set; }
    
    public Index(ICropRepository crops, ICropComponentRepository<Actuator> actuators, ICropComponentPropertyRepository<Command> commands)
        : base(crops)
    {
        _actuators = actuators;
        _commands = commands;
    }
    
    public async Task<IActionResult> OnGet(int farmingCompanyId, int cropId, int actuatorId)
    {
        Actuator = await _actuators.Get(farmingCompanyId, cropId, actuatorId);
        Commands = _commands.GetAll(farmingCompanyId, cropId, actuatorId).ToBlockingEnumerable();
        
        return Page();
    }

    public async Task<IActionResult> OnPostDelete(int farmingCompanyId, int cropId, int actuatorId)
    {
        await _actuators.Delete(farmingCompanyId, cropId, actuatorId);
        return RedirectToPage("/FarmManager/Crops/View", new {farmingCompanyId, cropId});
    }

}