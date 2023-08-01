using Microsoft.AspNetCore.Mvc;
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

MapCommonRoutes<FarmingCompany>("farmingCompanies");
MapCommonRoutes<WaterCompany>("waterCompanies");
/*MapCropsRoutes();
MapCropComponentsRoutes<Actuator>("actuators");
MapCropComponentsRoutes<Sensor>("sensors");*/

app.Run();

void InjectRepositories(IServiceCollection services, IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("(default)");

    var options = new DbContextOptionsBuilder<DataContext>()       // DataContext e' definita in OnlyFarms.Core
        .UseSqlite(connectionString)
        .Options;

    var dataContext = new DataContext(options);
    //dataContext.Database.EnsureCreated();     // rimosso perche' ri-creava il DB ogni volta che si provava ad eseguire una migrazione, comportando poi degli errori (Tabelle gia' esistenti) quando si eseguiva il comando "database update"

    services.AddScoped<DataContext>(_ => dataContext);

    // TODO rivedere questa parte, non so se vadano aggiunte tutte queste repository o meno (o se ne servano altre) --> e' meglio aggiungere repository di tipo diverso in base all'entita' coinvolta (vedi DotNetShop come esempio)
    // molte di queste entita' NON devono implementare tutte le operazioni (get, getAll, add, update e delete), quindi potrebbero essere necessarie delle repository diverse
    //services.AddScoped<IRepository<Actuator>>(_ => new DataContextRepository<Actuator>(dataContext));
    //services.AddScoped<IRepository<ActuatorCommand>>(_ => new DataContextRepository<ActuatorCommand>(dataContext));
    //services.AddScoped<IRepository<Crop>>(_ => new DataContextRepository<Crop>(dataContext));
    services.AddScoped<IRepository<FarmingCompany>>(_ => new DataContextRepository<FarmingCompany>(dataContext));
    //services.AddScoped<IRepository<Measurement>>(_ => new DataContextRepository<Measurement>(dataContext));
    //services.AddScoped<IRepository<Reservation>>(_ => new DataContextRepository<Reservation>(dataContext));
    //services.AddScoped<IRepository<Sensor>>(_ => new DataContextRepository<Sensor>(dataContext));
    services.AddScoped<IRepository<WaterCompany>>(_ => new DataContextRepository<WaterCompany>(dataContext));
    //services.AddScoped<IRepository<WaterUsage>>(_ => new DataContextRepository<WaterUsage>(dataContext));
}

// TODO per gli endpoint piu' "semplici" usare un metodo generico di mapping delle routes (come in DotNetShop)
// TODO aggiungere i metodi di documentazione swagger a tutti gli endpoint

void MapCommonRoutes<T>(string routeName) where T : class, IHasId
{
    var fullRoute = $"{API_PREFIX}/{ routeName }";
    var group = app.MapGroup(fullRoute)
        .WithTags(routeName);
    
    // GET all
    group.MapGet("/", ([FromServices] IRepository<T> repository) => repository.GetAll());

    // GET single
    group.MapGet("/{id:int}", async ([FromServices] IRepository<T> repository, int id) =>
    {
        var res = await repository.Get(id);
        return res == null ? ResourceNotFound<T>(id) : Results.Ok(res);
    });

    // POST
    group.MapPost("/", async ([FromServices] IRepository<T> repository, [FromBody] T newResource) =>
    {
        var res = await repository.Add(newResource);
        return res == null ? MissingParameters<T>() : ResourceCreated(fullRoute, res);     // Results.Created imposta nell'Header della Response anche la locazione della nuova risorsa
    });

    // PUT
    group.MapPut("/{id:int}", async ([FromServices] IRepository<T> repository, [FromRoute] int id, [FromBody] T updatedResource) =>
    {
        var res = await repository.Update(id, updatedResource);
        return res == null ? ResourceNotFound<T>(id) : Results.Ok(res);
    });

    // DELETE
    group.MapDelete("/{id:int}", async ([FromServices] IRepository<T> repository, [FromRoute] int id) =>
    {
        var res = await repository.Delete(id);
        return res == null ? ResourceNotFound<T>(id) : Results.Ok(res);
    });
}

// TODO ripensare a tutte le chiavi composte, vedi https://learn.microsoft.com/en-us/ef/core/modeling/keys?tabs=data-annotations e rigenerare una migrazione del DB
/*
void MapCropsRoutes()
{
    const string fullRoute = $"{ API_PREFIX }/farmingCompanies/{{companyId:int}}/crops";
    var group = app.MapGroup(fullRoute)
        .WithTags("crops");
    
    // GET all
    group.MapGet("/", async ([FromServices] IRepository<FarmingCompany> repository, [FromRoute] int companyId) =>
    {
        var company = await repository.Get(companyId);
        return company == null ? ResourceNotFound<FarmingCompany>(companyId) : Results.Ok(company.Crops);
    });
    
    // GET single
    group.MapGet("/{id:int}", async ([FromServices] IRepository<FarmingCompany> repository, [FromRoute] int companyId, [FromRoute] int id) =>
    {
        var company = await repository.Get(companyId);
        if (company == null)
        {
            return ResourceNotFound<FarmingCompany>(companyId);
        }

        var crop = company.Crops.Find(id);
        return crop == null ? ResourceNotFound<Crop>(id) : Results.Ok(company.Crops.Find(id));
    });
    
    // POST
}

// TODO creare delle routes generiche per Attuatori e Sensori (ad entrambe serve l'ID di un azienda agricola e di un campo)
void MapCropComponentsRoutes<T>(string routeName) where T : class, IHasId
{
    var fullRoute = $"{API_PREFIX}/farmingCompanies/{{companyId:int}}/crops/{{cropId:int}}/{routeName}";
    var group = app.MapGroup((fullRoute))
        .WithTags(routeName);
    
    // GET all
    group.MapGet("/", async ([FromServices] IRepository<FarmingCompany> repository, [FromRoute] int companyId, [FromRoute] int cropId) =>
    {
        var company = await repository.Get(companyId);
        return GetCropComponents<T>(company, companyId, cropId, null);  // con id = null viene restituita una lista (anche vuota) di risorse
    });
    
    // GET single
    group.MapGet("/{id:int}", async ([FromServices] IRepository<FarmingCompany> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int id) =>
    {
        var company = await repository.Get(companyId);
        return GetCropComponents<T>(company, companyId, cropId, id);    // con id valorizzato viene restituita una singola risorsa
    });
    
    // POST
    
    // DELETE
}

IResult GetCropComponents<T>(FarmingCompany? company, int companyId, int cropId, int? id)
{
    if (company == null)
    {
        return ResourceNotFound<FarmingCompany>(companyId);
    }

    var crop = company.Crops.Find(cropId);
    if (crop == null)
    {
        return ResourceNotFound<Crop>(cropId);
    }
    
    // sfruttata switched expression + pattern matching (sul tipo dell'oggetto generico T)
    return typeof(T) switch
    {
        // se viene passato un ID si restituisce un un'unica istanza della risorsa cercata, altrimenti si restituisce la lista completa
        // TODO ma se non esiste la risorsa non viene lanciato un errore 404!
        var type when type == typeof(Actuator) => id == null ? Results.Ok(crop.Actuators) : Results.Ok(crop.Actuators.Find((int) id)),
        var type when type == typeof(Sensor) => id == null ? Results.Ok(crop.Sensors) : Results.Ok(crop.Sensors.Find((int) id)),
        _ => Results.StatusCode(500)
    };
}*/

IResult ResourceCreated<T>(string route, T resource) where T : IHasId => Results.Created($"{ route }/{ resource.Id }", resource);
IResult ResourceNotFound<T>(int id) => Results.NotFound($"no resource of type '{ typeof(T).Name }' with ID = { id }");
IResult MissingParameters<T>() => Results.BadRequest($"missing parameters for a resource of type { nameof(T) }");       // in realta' questa funzione non viene mai lanciata, perche' in caso di parametri mancanti dotnet lancia in automatico un'eccezione con stato 500 o 505 con la lista di parametri mancanti (oltre allo stack di eccezioni