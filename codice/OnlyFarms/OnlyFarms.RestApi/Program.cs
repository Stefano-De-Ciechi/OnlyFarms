using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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

// gestore delle eccezioni "custom" per intercettare NotFoundException (lanciate dalle varie repository dei dati)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        
        var errorMessage = new ErrorMessage()
        {
            Message = "Error during the current request",
            Resource = context.Request.Path.ToString()
        };

        if ((bool) exceptionHandlerPathFeature?.Error.GetType().IsGenericType)      // le eccezioni di tipo NotFoundException sono generiche
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            errorMessage.Message = exceptionHandlerPathFeature.Error.Message;
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            errorMessage.Message = exceptionHandlerPathFeature.Error.Message;
        }

        await context.Response.WriteAsJsonAsync(errorMessage);
    });
});

app.UseHttpsRedirection();

MapCommonRoutes<FarmingCompany>("farmingCompanies");
MapCommonRoutes<WaterCompany>("waterCompanies");
MapCropsRoutes();
MapCropComponentsRoutes<Actuator>("actuators");
MapCropComponentsRoutes<Sensor>("sensors");
MapCropComponentsPropertiesRoutes<Actuator, Command>("commands");
MapCropComponentsPropertiesRoutes<Sensor, Measurement>("measurements");

app.Run();

// metodo creato per semplificare la scrittura della dependency injection per repository che necessitano di altre repository gia' create
TService GetService<TService>(IServiceProvider services) where TService : notnull
{
    return services.GetRequiredService<TService>();
}

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
    
    services.AddScoped<IRepository<FarmingCompany>>(_ => new DataContextRepository<FarmingCompany>(dataContext));
    services.AddScoped<IRepository<WaterCompany>>(_ => new DataContextRepository<WaterCompany>(dataContext));
    services.AddScoped<ICropRepository>(serviceProvider => new CropRepository(dataContext, GetService<IRepository<FarmingCompany>>(serviceProvider)));
    services.AddScoped<ICropComponentRepository<Actuator>>(serviceProvider => new CropComponentRepository<Actuator>(dataContext, GetService<ICropRepository>(serviceProvider)));
    services.AddScoped<ICropComponentRepository<Sensor>>(serviceProvider => new CropComponentRepository<Sensor>(dataContext, GetService<ICropRepository>(serviceProvider)));
    services.AddScoped<ICropComponentPropertyRepository<Command>>(serviceProvider => new CropComponentPropertyRepository<Actuator, Command>(dataContext, GetService<ICropComponentRepository<Actuator>>(serviceProvider)));
    services.AddScoped<ICropComponentPropertyRepository<Measurement>>(serviceProvider => new CropComponentPropertyRepository<Sensor, Measurement>(dataContext, GetService<ICropComponentRepository<Sensor>>(serviceProvider)));
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
        var res = await repository.Get(id);
        return Results.Ok(res);
    });

    // POST
    group.MapPost("/", async ([FromServices] IRepository<T> repository, [FromBody] T newResource) =>
    {
        var res = await repository.Add(newResource);
        return ResourceCreated(fullRoute, res); // Results.Created imposta nell'Header della Response anche la locazione della nuova risorsa
    });

    // PUT
    group.MapPut("/{id:int}", async ([FromServices] IRepository<T> repository, [FromRoute] int id, [FromBody] T updatedResource) =>
    {
        var res = await repository.Update(id, updatedResource);
        return Results.Ok(res);
    });

    // DELETE
    group.MapDelete("/{id:int}", async ([FromServices] IRepository<T> repository, [FromRoute] int id) =>
    {
        var res = await repository.Delete(id);
        return Results.Ok(res);
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
        var res = repository.GetAll(companyId);
        return Results.Ok(res);
    });

    // GET single
    group.MapGet("/{id:int}", async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromRoute] int id) =>
    {
        var crop = await repository.Get(companyId, id);
        return Results.Ok(crop);
    });
    
    // POST
    group.MapPost("/", async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromBody] Crop crop) =>
    {
        var res = await repository.Add(companyId, crop);
        return ResourceCreated(fullRoute, res);
    });
    
    // PUT
    group.MapPut("/{id:int}", async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromRoute] int id, [FromBody] Crop updatedCrop) =>
    {
        var res = await repository.Update(companyId, id, updatedCrop);
        return Results.Ok(res);
    });
    
    // DELETE
    group.MapDelete("/{id:int}", async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromRoute] int id) =>
    {
        var res = await repository.Delete(companyId, id);
        return Results.Ok(res);
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
        var res = repository.GetAll(companyId, cropId);
        return Results.Ok(res);
    });
    
    // GET single
    group.MapGet("/{id:int}", async ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int id) =>
    {
        var res = await repository.Get(companyId, cropId, id);
        return Results.Ok(res);
    });
    
    // POST
    group.MapPost("/", async ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromBody] T component) =>
    {
        var res = await repository.Add(companyId, cropId, component);
        return ResourceCreated(fullRoute, res);
    });
    
    // DELETE
    group.MapDelete("/{id:int}", async ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int id) =>
    {
        var res = await repository.Delete(companyId, cropId, id);
        return Results.Ok(res);
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
        var res = repository.GetAll(companyId, cropId, componentId);
        return Results.Ok(res);
    });
    
    // GET single
    group.MapGet("/{id:int}", async ([FromServices] ICropComponentPropertyRepository<CP> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int componentId, [FromRoute] int id) =>
    {
        var res = await repository.Get(companyId, cropId, componentId, id);
        return Results.Ok(res);
    });
    
    // POST
    group.MapPost("/", async ([FromServices] ICropComponentPropertyRepository<CP> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int componentId, [FromBody] CP component) =>
    {
        var res = await repository.Add(companyId, cropId, componentId, component);
        return ResourceCreated(fullRoute, res);
    });
}

IResult ResourceCreated<T>(string route, T resource) where T : IHasId => Results.Created($"{ route }/{ resource.Id }", resource);