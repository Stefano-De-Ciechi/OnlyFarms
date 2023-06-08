using Microsoft.EntityFrameworkCore;

const string API_VERSION = "v1";
const string API_PREFIX = $"api/{ API_VERSION }";

var builder = WebApplication.CreateBuilder(args);

InjectRepositories(builder.Services, builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(API_VERSION, new Microsoft.OpenApi.Models.OpenApiInfo { Title = "OnlyFarms API", Version = API_VERSION });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

MapActuatorsRoutes();

app.Run();

void InjectRepositories(IServiceCollection services, ConfigurationManager configuration)
{
    var connectionString = configuration.GetConnectionString("(default)");

    var options = new DbContextOptionsBuilder<DataContext>()       // DataContext e' definita in OnlyFarms.Core
        .UseSqlite(connectionString)
        .Options;

    var dataContext = new DataContext(options);
    //dataContext.Database.EnsureCreated();     // rimosso perche' ri-creava il DB ogni volta che si provava ad eseguire una migrazione, comportando poi degli errori (Tabelle gia' esistenti) quando si eseguiva il comando "database update"

    services.AddScoped<DataContext>(_ => dataContext);

    // TODO rivedere questa parte, non so se vadano aggiunte tutte queste repository o meno (o se ne servano altre)
    // molte di queste entita' NON devono implementare tutte le operazioni (get, getAll, add, update e delete), quindi potrebbero essere necessarie delle repository diverse
    services.AddScoped<IRepository<Actuator>>(_ => new DataContextRepository<Actuator>(dataContext));
    services.AddScoped<IRepository<ActuatorCommand>>(_ => new DataContextRepository<ActuatorCommand>(dataContext));
    services.AddScoped<IRepository<Crop>>(_ => new DataContextRepository<Crop>(dataContext));
    services.AddScoped<IRepository<FarmingCompany>>(_ => new DataContextRepository<FarmingCompany>(dataContext));
    services.AddScoped<IRepository<Measurement>>(_ => new DataContextRepository<Measurement>(dataContext));
    services.AddScoped<IRepository<Reservation>>(_ => new DataContextRepository<Reservation>(dataContext));
    services.AddScoped<IRepository<Sensor>>(_ => new DataContextRepository<Sensor>(dataContext));
    services.AddScoped<IRepository<WaterCompany>>(_ => new DataContextRepository<WaterCompany>(dataContext));
    services.AddScoped<IRepository<WaterUsage>>(_ => new DataContextRepository<WaterUsage>(dataContext));
}

// TODO trovare un approccio scalabile per gli endpoint complessi come questo
void MapActuatorsRoutes()
{
    var group = app.MapGroup($"{ API_PREFIX }/waterCompanies/{{:companyId}}/crops/{{:cropId}}/actuators")
        .WithTags("Actuators");

    group.MapGet("/{:id}", (DataContext repository, int companyId, int cropId, int actuatorId) =>
    {
        // TODO in questo caso, IRepository<Actuator> e' troppo generico, serve un repository custom in cui si passano gli id dell'azienda, della coltivazione e dell'attuatore e si cerca nel DB solo nelle tabelle giuste
        // qualcosa del tipo _context.WaterCompanies.Get(companyId).Crops.Get(cropId).Get(actuatorId);
        return repository.FarmingCompanies?.Find(companyId)?.Crops?.Find(cropId)?.Actuators.Find(actuatorId);
    });
}