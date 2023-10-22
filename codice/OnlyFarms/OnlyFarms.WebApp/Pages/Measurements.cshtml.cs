using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages
{
    public class MeasurementsModel : PageModel
    {
        private readonly ICropComponentRepository<Actuator> _actuatorComponent;
        private readonly ICropComponentRepository<Sensor> _sensorComponent;
        
        public MeasurementsModel(ICropComponentRepository<Actuator> actuatorComponent, ICropComponentRepository<Sensor> sensorComponent)
        {
            _actuatorComponent = actuatorComponent;
            _sensorComponent = sensorComponent;
        
        }

        public IAsyncEnumerable<Actuator>? Actuators;
        public IAsyncEnumerable<Sensor>? Sensors;

        public void OnGet(int farmingCompanyId, int cropId)
        {
            Actuators = _actuatorComponent.GetAll(farmingCompanyId, cropId);
            Sensors = _sensorComponent.GetAll(farmingCompanyId, cropId);

        }
    }
}