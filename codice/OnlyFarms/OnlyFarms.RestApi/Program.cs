using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlyFarms.Core.Data;
using OnlyFarms.Core.Infrastructure;
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
MapCropComponentsPropertiesRoutes<Actuator, ActuatorCommand>("actuatorCommands");
MapCropComponentsPropertiesRoutes<Sensor, Measurement>("measurements");

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
    
    //services.AddScoped<IRepository<Reservation>>(_ => new DataContextRepository<Reservation>(dataContext));
    //services.AddScoped<IRepository<WaterUsage>>(_ => new DataContextRepository<WaterUsage>(dataContext));
    
    // TODO gestire eccezioni in tutte le repository come in ICropComponentPropertyRepository
    services.AddScoped<IRepository<FarmingCompany>>(_ => new DataContextRepository<FarmingCompany>(dataContext));
    services.AddScoped<IRepository<WaterCompany>>(_ => new DataContextRepository<WaterCompany>(dataContext));
    services.AddScoped<ICropRepository>(_ => new CropRepository(dataContext));
    services.AddScoped<ICropComponentRepository<Actuator>>(_ => new CropComponentRepository<Actuator>(dataContext));
    services.AddScoped<ICropComponentRepository<Sensor>>(_ => new CropComponentRepository<Sensor>(dataContext));
    services.AddScoped<ICropComponentPropertyRepository<ActuatorCommand>>(serviceProvider => new CropComponentPropertyRepository<Actuator, ActuatorCommand>(dataContext, serviceProvider.GetRequiredService<ICropComponentRepository<Actuator>>()));
    services.AddScoped<ICropComponentPropertyRepository<Measurement>>(serviceProvider => new CropComponentPropertyRepository<Sensor, Measurement>(dataContext, serviceProvider.GetRequiredService<ICropComponentRepository<Sensor>>()));
}

// TODO aggiungere i metodi di documentazione swagger a tutti gli endpoint
// TODO ripensare a tutte le chiavi composte, vedi https://learn.microsoft.com/en-us/ef/core/modeling/keys?tabs=data-annotations e rigenerare una migrazione del DB

void MapCommonRoutes<T>(string routeName) where T : class, IHasId
{
    var fullRoute = $"{API_PREFIX}/{ routeName }";
    var group = app.MapGroup(fullRoute)
        .WithTags(routeName.Capitalize());
    
    // GET all
    group.MapGet("/", ([FromServices] IRepository<T> repository) => repository.GetAll());

    // GET single
    group.MapGet("/{id:int}", async ([FromServices] IRepository<T> repository, int id) =>
    {
        try
        {
            var res = await repository.Get(id);
            return Results.Ok(res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });

    // POST
    group.MapPost("/", async ([FromServices] IRepository<T> repository, [FromBody] T newResource) =>
    {
        try
        {
            var res = await repository.Add(newResource);
            return ResourceCreated(fullRoute, res); // Results.Created imposta nell'Header della Response anche la locazione della nuova risorsa
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });

    // PUT
    group.MapPut("/{id:int}", async ([FromServices] IRepository<T> repository, [FromRoute] int id, [FromBody] T updatedResource) =>
    {
        try
        {
            var res = await repository.Update(id, updatedResource);
            return Results.Ok(res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });

    // DELETE
    group.MapDelete("/{id:int}", async ([FromServices] IRepository<T> repository, [FromRoute] int id) =>
    {
        try
        {
            var res = await repository.Delete(id);
            return Results.Ok(res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });
}

void MapCropsRoutes()
{
    const string fullRoute = $"{ API_PREFIX }/farmingCompanies/{{companyId:int}}/crops";
    var group = app.MapGroup(fullRoute)
        .WithTags("Crops");
    
    // GET all
    group.MapGet("/", ([FromServices] ICropRepository repository, [FromRoute] int companyId) =>
    {
        try
        {
            var res = repository.GetAll(companyId);
            return Results.Ok(res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });

    // GET single
    group.MapGet("/{id:int}", async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromRoute] int id) =>
    {
        try
        {
            var crop = await repository.Get(companyId, id);
            return Results.Ok(crop);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });
    
    // POST
    group.MapPost("/", async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromBody] Crop crop) =>
    {
        try
        {
            var res = await repository.Add(companyId, crop); // TODO gestire eccezione
            return ResourceCreated(fullRoute, res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });
    
    // PUT
    group.MapPut("/{id:int}", async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromRoute] int id, [FromBody] Crop updatedCrop) =>
    {
        try
        {
            var res = await repository.Update(companyId, id, updatedCrop);
            return Results.Ok(res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });
    
    // DELETE
    group.MapDelete("/{id:int}", async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromRoute] int id) =>
    {
        try
        {
            var res = await repository.Delete(companyId, id);
            return Results.Ok(res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });
}

void MapCropComponentsRoutes<T>(string routeName) where T : class, ICropComponent
{
    var fullRoute = $"{API_PREFIX}/farmingCompanies/{{companyId:int}}/crops/{{cropId:int}}/{routeName}";
    var group = app.MapGroup(fullRoute)
        .WithTags(routeName.Capitalize());
    
    // GET all
    group.MapGet("/", ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId) =>
    {
        try
        {
            var res = repository.GetAll(companyId, cropId);
            return Results.Ok(res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
        
    });
    
    // GET single
    group.MapGet("/{id:int}", async ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int id) =>
    {
        try
        {
            var res = await repository.Get(companyId, cropId, id);
            return Results.Ok(res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });
    
    // POST
    group.MapPost("/", async ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromBody] T component) =>
    {
        try
        {
            var res = await repository.Add(companyId, cropId, component); // TODO gestire eccezione
            return ResourceCreated(fullRoute, res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });
    
    // DELETE
    group.MapDelete("/{id:int}", async ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int id) =>
    {
        try
        {
            var res = await repository.Delete(companyId, cropId, id);
            return Results.Ok(res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });
}

void MapCropComponentsPropertiesRoutes<C, CP>(string routeName) where C : class, IHasId, ICropComponent where CP : class, IHasId, ICropComponentProperty
{
    var fullRoute = $"{API_PREFIX}/farmingCompanies/{{companyId:int}}/crops/{{cropId:int}}/{ typeof(C).Name.UnCapitalize() }s/{{componentId:int}}/{ routeName }";
    var group = app.MapGroup(fullRoute)
        .WithTags(routeName.Capitalize());

    // GET all
    group.MapGet("/", ([FromServices] ICropComponentPropertyRepository<CP> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int componentId) =>
    {
        try
        {
            var res = repository.GetAll(companyId, cropId, componentId);
            return Results.Ok(res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });
    
    // GET single
    group.MapGet("/{id:int}", async ([FromServices] ICropComponentPropertyRepository<CP> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int componentId, [FromRoute] int id) =>
    {
        try
        {
            var res = await repository.Get(companyId, cropId, componentId, id);
            return Results.Ok(res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });
    
    // POST
    group.MapPost("/", async ([FromServices] ICropComponentPropertyRepository<CP> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int componentId, [FromBody] CP component) =>
    {
        try
        {
            var res = await repository.Add(companyId, cropId, componentId, component);
            return ResourceCreated(fullRoute, res);
        }
        catch (KeyNotFoundException e)
        {
            return ResourceNotFound(e);
        }
    });
}

IResult ResourceCreated<T>(string route, T resource) where T : IHasId => Results.Created($"{ route }/{ resource.Id }", resource);
IResult ResourceNotFound(KeyNotFoundException e) => Results.NotFound(e.Message);     // TODO rinominare