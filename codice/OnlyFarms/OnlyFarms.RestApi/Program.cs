using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyFarms.Core.Data;
using OnlyFarms.Core.Models;
using OnlyFarms.RestApi.Data;

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
MapCropsRoutes();
MapCropComponentsRoutes<Actuator>("actuators");
MapCropComponentsRoutes<Sensor>("sensors");
/*MapCropComponentsRoutes<Actuator>("actuators");
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
    //services.AddScoped<IRepository<ActuatorCommand>>(_ => new DataContextRepository<ActuatorCommand>(dataContext));
    //services.AddScoped<IRepository<Measurement>>(_ => new DataContextRepository<Measurement>(dataContext));
    //services.AddScoped<IRepository<Reservation>>(_ => new DataContextRepository<Reservation>(dataContext));
    //services.AddScoped<IRepository<WaterUsage>>(_ => new DataContextRepository<WaterUsage>(dataContext));
    
    services.AddScoped<IRepository<FarmingCompany>>(_ => new DataContextRepository<FarmingCompany>(dataContext));
    services.AddScoped<IRepository<WaterCompany>>(_ => new DataContextRepository<WaterCompany>(dataContext));
    services.AddScoped<ICropRepository>(_ => new CropRepository(dataContext));
    services.AddScoped<ICropComponentRepository<Actuator>>(_ => new CropComponentRepository<Actuator>(dataContext));
    services.AddScoped<ICropComponentRepository<Sensor>>(_ => new CropComponentRepository<Sensor>(dataContext));

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

void MapCropsRoutes()
{
    const string fullRoute = $"{ API_PREFIX }/farmingCompanies/{{companyId:int}}/crops";
    var group = app.MapGroup(fullRoute)
        .WithTags("crops");
    
    // GET all
    group.MapGet("/", ([FromServices] ICropRepository repository, [FromRoute] int companyId) => repository.GetAll(companyId));

    // GET single
    group.MapGet("/{id:int}", async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromRoute] int id) =>
    {
        var crop = await repository.Get(companyId, id);
        return crop == null ? ResourceNotFound<Crop>(id) : Results.Ok(crop);
    });
    
    // POST
    group.MapPost("/", async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromBody] Crop crop) =>
    {
        var res = await repository.Add(companyId, crop);    // TODO gestire eccezione
        return res == null ? MissingParameters<Crop>() : ResourceCreated(fullRoute, res);
    });
    
    // PUT
    group.MapPut("/{id:int}", async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromRoute] int id, [FromBody] Crop updatedCrop) =>
    {
        var res = await repository.Update(companyId, id, updatedCrop);
        return res == null ? ResourceNotFound<Crop>(id) : Results.Ok(res);
    });
    
    // DELETE
    group.MapDelete("/{id:int}", async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromRoute] int id) =>
    {
        var res = await repository.Delete(companyId, id);
        return res == null ? ResourceNotFound<Crop>(id) : Results.Ok(res);
    });
}

void MapCropComponentsRoutes<T>(string routeName) where T : class, ICropComponent
{
    var fullRoute = $"{API_PREFIX}/farmingCompanies/{{companyId:int}}/crops/{{cropId:int}}/{routeName}";
    var group = app.MapGroup((fullRoute))
        .WithTags(routeName);
    
    // GET all
    group.MapGet("/", ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId) => repository.GetAll(companyId, cropId));
    
    // GET single
    group.MapGet("/{id:int}", async ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int id) =>
    {
        var res = await repository.Get(companyId, cropId, id);
        return res == null ? ResourceNotFound<T>(id) : Results.Ok(res);
    });
    
    // POST
    group.MapPost("/", async ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromBody] T component) =>
    {
        try
        {
            var res = await repository.Add(companyId, cropId, component); // TODO gestire eccezione
            return res == null ? MissingParameters<T>() : ResourceCreated(fullRoute, res);
        }
        catch (KeyNotFoundException e)
        {
            return Results.NotFound(e.Message);
        }
        
    });
    
    // DELETE
    group.MapDelete("/{id:int}", async ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int id) =>
    {
        var res = await repository.Delete(companyId, cropId, id);
        return res == null ? ResourceNotFound<Crop>(id) : Results.Ok(res);
    });
}

IResult ResourceCreated<T>(string route, T resource) where T : IHasId => Results.Created($"{ route }/{ resource.Id }", resource);
IResult ResourceNotFound<T>(int id) => Results.NotFound($"no resource of type '{ typeof(T).Name }' with ID = { id }");
//IResult ResourceNotFound(KeyNotFoundException e) => Results.NotFound(e.Message);
IResult MissingParameters<T>() => Results.BadRequest($"missing parameters for a resource of type { nameof(T) }");       // in realta' questa funzione non viene mai lanciata, perche' in caso di parametri mancanti dotnet lancia in automatico un'eccezione con stato 500 o 505 con la lista di parametri mancanti (oltre allo stack di eccezioni