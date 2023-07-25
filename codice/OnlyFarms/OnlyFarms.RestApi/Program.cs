using Microsoft.AspNetCore.Http.HttpResults;
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

MapActuatorsRoutes();
MapFarmingCompaniesRoutes();
MapWaterCompaniesRoutes();

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

// TODO per gli endpoint piu' "semplici" usare un metodo generico di mapping delle routes (come in DotNetShop)

void MapActuatorsRoutes()
{
    var group = app.MapGroup($"{ API_PREFIX }/farmingCompanies/{{companyId:int}}/crops/{{cropId:int}}/actuators")
        .WithTags("Actuators");

    group.MapGet("/", async ([FromServices] IRepository<FarmingCompany> repository, [FromRoute] int companyId, [FromRoute] int cropId) =>
    {
        var farmingCompany = await repository.Get(companyId);

        if (farmingCompany == null)
        {
            return Results.BadRequest($"no farming company with ID = {companyId}");
        }

        var crop = farmingCompany.Crops.Find(cropId);

        return crop == null ? Results.BadRequest($"no crops with ID = {cropId}") : Results.Ok(crop.Actuators);
    })
        .Produces(200)
        .Produces(400);

    group.MapGet("/{actuatorId:int}", async ([FromServices] IRepository<FarmingCompany> repository, [FromRoute] int companyId,  [FromRoute] int cropId, [FromRoute] int actuatorId) =>
    {
        var farmingCompany = await repository.Get(companyId);

        if (farmingCompany == null)
        {
            return Results.BadRequest($"no farming company with ID = { companyId }");
        }

        var crop = farmingCompany.Crops.Find(cropId);

        return crop == null ? Results.BadRequest($"no crops with ID = { cropId }") : Results.Ok(crop.Actuators.Find(actuatorId));
    })
        .Produces(200)
        .Produces(400);
    
    group.MapPost("/", async ([FromServices] IRepository<Actuator> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromBody] Actuator actuator) =>
    {
        // TODO cercare di capire come avviene il collegamento tra questo attuatore e l'azienda e coltivazione passate come parametri
        var res = await repository.Add(new Actuator { Tag = actuator.Tag });      // TODO ritornare l'ID dell'attuatore appena creato
        return res;
    });
}

void MapFarmingCompaniesRoutes()
{
    var group = app.MapGroup($"{API_PREFIX}/farmingCompanies")
        .WithTags("Farming Companies");

    group.MapGet("/", ([FromServices] IRepository<FarmingCompany> repository) => repository.GetAll());

    group.MapGet("/{id:int}", async ([FromServices] IRepository<FarmingCompany> repository, [FromRoute] int id) =>
    {
        var fc = await repository.Get(id);

        return fc == null ? Results.NotFound($"no farming company with ID = {id}") : Results.Ok(fc);
    });

    group.MapPost("/", async ([FromServices] IRepository<FarmingCompany> repository, [FromBody] FarmingCompany fc) =>
    {
        var id = await repository.Add(new FarmingCompany { Address = fc.Address, Name = fc.Name, WaterSupply = fc.WaterSupply});
        return id;
    })
        .Produces<FarmingCompany>(201);
}

void MapWaterCompaniesRoutes()
{
    var group = app.MapGroup($"{ API_PREFIX }/waterCompanies")
        .WithTags("Water Companies");

    group.MapGet("/", ([FromServices] IRepository<WaterCompany> repository) => repository.GetAll());

    group.MapGet("/{id:int}", async ([FromServices] IRepository<WaterCompany> repository, [FromRoute] int id) =>
    {
        var wc = await repository.Get(id);

        return wc == null ? Results.NotFound($"no water company with ID = { id }") : Results.Ok(wc);
    });

    group.MapPost("/", async ([FromServices] IRepository<WaterCompany> repository, [FromBody] WaterCompany wc) =>
    {
        var res = await repository.Add(new WaterCompany { Address = wc.Address, Name = wc.Name, WaterSupply = wc.WaterSupply });

        return Results.Created($"{ API_PREFIX }/waterCompanies/{ res?.Id }", res);      // Results.Created restituisce nel body i valori della nuova risorsa e nell'header aggiunge un parametro "location" che punta all'url della nuova risorsa
    })
        .Produces<WaterCompany>(201);
}