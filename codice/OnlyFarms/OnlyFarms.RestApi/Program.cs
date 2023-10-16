using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

const string API_VERSION = "v1";
const string API_PREFIX = $"api/{ API_VERSION }";

var builder = WebApplication.CreateBuilder(args);

InjectRepositories(builder.Services, builder.Configuration);

#region ===== Authentication and Authorization Setup =====

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(API_VERSION, new Microsoft.OpenApi.Models.OpenApiInfo { Title = "OnlyFarms API", Version = API_VERSION });
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

/* TODO aggiungere autenticazione e autorizzazione, vedi link:
    - https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/security?view=aspnetcore-7.0
    - https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-7.0     questa e' policy-based, se si vuole role-based o claim-based guardare nei link sulla sinistra
*/
// TODO questa parte e' da de-commentare quando si implementa la generazione di Token JWT nella WebApp
/*builder.Services.AddAuthentication("Bearer").AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new()
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey((Encoding.ASCII.GetBytes(builder.Configuration["JwtSecretKey"]!)))  // TODO ricordarsi di cambiare il valore della key quando si implementa la generazione di Token JWT nella WebApp
    };
});*/

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer()
    .AddJwtBearer("LocalAuthIssuer");

builder.Services.AddAuthorization();

// TODO rivedere sta parte, sopratutto sui claim assegnati
// TODO magari serve sfruttare un'implementazione di IAuthorizationHandler
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Roles.Admin, policy =>
    {
        policy.RequireRole(Roles.Admin);
    })
    
    .AddPolicy(Roles.FarmManager, policy =>
    {
        policy.RequireRole(Roles.FarmManager);
    })
    
    .AddPolicy(Roles.WaterManager, policy =>
    {
        policy.RequireRole(Roles.WaterManager);
    })
    
    .AddPolicy(Roles.IoTSubSystem, policy =>
    {
        policy.RequireRole(Roles.IoTSubSystem);
    })
    
    .AddPolicy("AnyAuthUser", policy =>
    {
        policy.RequireAssertion(context => (context.User.IsInRole(Roles.Admin) || context.User.IsInRole(Roles.FarmManager) || context.User.IsInRole(Roles.WaterManager)));
    })
    
    /*
        spiegazione delle due policy distinte:
        per avere un metodo generico MapCompanyRoutes<T> che funzioni con entrambi i tipi di azienda, c'e' bisogno di un meccanismo
        per definire quali autorizzazioni devono essere possedute dall'utente, che sono comuni per i due metodi GET All e GET, ma
        differiscono per i metodi POST, PUT e DELETE in cui un utente del tipo FarmManager NON puo' agire su compagnie di tipo
        WaterCompany e viceversa; per fare questo, in tali verbi e' applicato il metodo .RequireAuthorization("<policy name>") che, 
        a tempo di compilazione (quando i metodi generici vengono "espansi" in metodi con i giusti tipi), controlla quale tipo di azienda
        si sta mappando e passa la policy corretta da applicare
    */
    .AddPolicy("FarmingCompanyPolicy", policy =>
    {
        policy.RequireAssertion(context => (context.User.IsInRole(Roles.Admin) || context.User.IsInRole(Roles.FarmManager)));
    })
    
    .AddPolicy("WaterCompanyPolicy", policy =>
    {
        policy.RequireAssertion(context => (context.User.IsInRole(Roles.Admin) || context.User.IsInRole(Roles.WaterManager)));
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region ===== Custom Exception Handler =====
// gestore delle eccezioni "custom" per intercettare NotFoundException (lanciate dalle varie repository dei dati) e restituire le eccezioni in formato JSON
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

#endregion

#region ===== App Setup and Routes Mapping =====
app.UseHttpsRedirection();

MapCompanyRoutes<FarmingCompany>("farmingCompanies");
MapCompanyRoutes<WaterCompany>("waterCompanies");
MapCropsRoutes();
MapCropComponentsRoutes<Actuator>("actuators");
MapCropComponentsRoutes<Sensor>("sensors");
MapCropComponentsPropertiesRoutes<Actuator, Command>("commands");
MapCropComponentsPropertiesRoutes<Sensor, Measurement>("measurements");
MapWaterUsageRoutes();
MapReservationsRoutes();
MapWaterLimitRoutes();

#endregion

app.Run();

return;     // fine del programma che "serve" la REST API

#region ===== Custom Repositories =====

// metodo creato per semplificare la scrittura della dependency injection per repository che necessitano di altre repository gia' create
TService GetService<TService>(IServiceProvider services) where TService : notnull
{
    return services.GetRequiredService<TService>();
}

void InjectRepositories(IServiceCollection services, IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("(default)");

    var options = new DbContextOptionsBuilder<DataContext>()       // DataContext e' definita in OnlyFarms.Core
        .UseSqlite(connectionString, b => b.MigrationsAssembly("OnlyFarms.RestApi"))    // TODO quando si aggiungera' la parte OnlyFarms.WebApp, e' piu' logico spostare il database e le migrazioni in quel progetto!
        .Options;

    var dataContext = new DataContext(options);
    //dataContext.Database.EnsureCreated();     // rimosso perche' ri-creava il DB ogni volta che si provava ad eseguire una migrazione, comportando poi degli errori (Tabelle gia' esistenti) quando si eseguiva il comando "database update"

    services.AddScoped<DataContext>(_ => dataContext);
    
    //services.AddScoped<ICompanyRepository<Reservation>>(_ => new CompanyRepository<Reservation>(dataContext));

    services.AddScoped<ICompanyRepository<FarmingCompany>>(_ => new CompanyRepository<FarmingCompany>(dataContext));
    services.AddScoped<ICompanyRepository<WaterCompany>>(_ => new CompanyRepository<WaterCompany>(dataContext));
    services.AddScoped<ICropRepository>(serviceProvider => new CropRepository(dataContext, GetService<ICompanyRepository<FarmingCompany>>(serviceProvider)));
    services.AddScoped<ICropComponentRepository<Actuator>>(serviceProvider => new CropComponentRepository<Actuator>(dataContext, GetService<ICropRepository>(serviceProvider)));
    services.AddScoped<ICropComponentRepository<Sensor>>(serviceProvider => new CropComponentRepository<Sensor>(dataContext, GetService<ICropRepository>(serviceProvider)));
    services.AddScoped<ICropComponentPropertyRepository<Command>>(serviceProvider => new CropComponentPropertyRepository<Actuator, Command>(dataContext, GetService<ICropComponentRepository<Actuator>>(serviceProvider)));
    services.AddScoped<ICropComponentPropertyRepository<Measurement>>(serviceProvider => new CropComponentPropertyRepository<Sensor, Measurement>(dataContext, GetService<ICropComponentRepository<Sensor>>(serviceProvider)));
    services.AddScoped<IWaterUsageRepository>(serviceProvider => new WaterUsageRepository(dataContext, GetService<ICompanyRepository<FarmingCompany>>(serviceProvider)));
    services.AddScoped<IReservationRepository>(serviceProvider => new ReservationRepository(dataContext, GetService<ICompanyRepository<FarmingCompany>>(serviceProvider), GetService<ICompanyRepository<WaterCompany>>(serviceProvider)));
    services.AddScoped<IWaterLimitRepository>(serviceProvider => new WaterLimitRepository(dataContext, GetService<ICompanyRepository<FarmingCompany>>(serviceProvider), GetService<ICompanyRepository<WaterCompany>>(serviceProvider)));
}

#endregion

// TODO ??? aggiornare le classi e le repository, modificando i campi come int ...ID con il tipo di classe a cui fanno riferimento (EF li rimpiazza con la propria chiave primaria in automatico, almeno credo; vedi WaterLimit)

#region ===== Custom Routes ===== 

// Routes per FarmingCompanies e WaterCompanies
void MapCompanyRoutes<T>(string routeName) where T : class, ICompany
{
    var fullRoute = $"{API_PREFIX}/{ routeName }";
    var group = app.MapGroup(fullRoute)
        .WithTags(routeName.Capitalize());
    
    // GET all
    group.MapGet("/", ([FromServices] ICompanyRepository<T> repository, [FromQuery] string? city) =>
    {
        var res = city == null ? repository.GetAll() : repository.GetAll(city);
        return Results.Ok(res);
    })
        .RequireAuthorization("AnyAuthUser")    /* endpoint accessibile da tutti i tipi di utente autenticati */
        .Produces<IAsyncEnumerable<T>>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403);
    
    // GET single
    group.MapGet("/{id:int}", async ([FromServices] ICompanyRepository<T> repository, int id) =>
    {
        var res = await repository.Get(id);
        return Results.Ok(res);
    })
        .RequireAuthorization("AnyAuthUser")
        .Produces<T>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);

    // POST
    group.MapPost("/", async ([FromServices] ICompanyRepository<T> repository, [FromBody] T newResource) =>
    {
        var res = await repository.Add(newResource);
        return ResourceCreated(fullRoute, res); // Results.Created imposta nell'Header della Response anche la locazione della nuova risorsa
    })
        .RequireAuthorization(typeof(T) == typeof(FarmingCompany) ? "FarmingCompanyPolicy" : "WaterCompanyPolicy")      /* la policy richiesta cambia a seconda del tipo generico che viene "rimpiazzato a T" a tempo di compilazione */
        .Produces<T>(201)
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403);

    // PUT
    group.MapPut("/{id:int}", async ([FromServices] ICompanyRepository<T> repository, [FromRoute] int id, [FromBody] T updatedResource) =>
    {
        var res = await repository.Update(id, updatedResource);
        return Results.Ok(res);
    })
        .RequireAuthorization(typeof(T) == typeof(FarmingCompany) ? "FarmingCompanyPolicy" : "WaterCompanyPolicy")
        .Produces<T>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);

    // DELETE
    group.MapDelete("/{id:int}", async ([FromServices] ICompanyRepository<T> repository, [FromRoute] int id) =>
    {
        var res = await repository.Delete(id);
        return Results.Ok(res);
    })
        .RequireAuthorization(typeof(T) == typeof(FarmingCompany) ? "FarmingCompanyPolicy" : "WaterCompanyPolicy")
        .Produces<T>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
}

void MapCropsRoutes()
{
    const string fullRoute = $"{ API_PREFIX }/farmingCompanies/{{companyId:int}}/crops";
    var group = app.MapGroup(fullRoute)
        .WithTags("Crops");
    
    // GET all
    group.MapGet("/",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager}")]
            ([FromServices] ICropRepository repository, [FromRoute] int companyId) =>
    {
        var res = repository.GetAll(companyId);
        return Results.Ok(res);
    })
        .Produces<IAsyncEnumerable<Crop>>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);

    // GET single
    group.MapGet("/{id:int}",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager}")]
            async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromRoute] int id) =>
    {
        var crop = await repository.Get(companyId, id);
        return Results.Ok(crop);
    })
        .Produces<Crop>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // POST
    group.MapPost("/",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager}")]
            async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromBody] Crop crop) =>
    {
        var res = await repository.Add(companyId, crop);
        return ResourceCreated(fullRoute, res);
    })
        .Produces<Crop>(201)
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // PUT
    group.MapPut("/{id:int}",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager}")]
            async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromRoute] int id, [FromBody] Crop updatedCrop) =>
    {
        var res = await repository.Update(companyId, id, updatedCrop);
        return Results.Ok(res);
    })
        .Produces<Crop>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // DELETE
    group.MapDelete("/{id:int}",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager}")]
            async ([FromServices] ICropRepository repository, [FromRoute] int companyId, [FromRoute] int id) =>
    {
        var res = await repository.Delete(companyId, id);
        return Results.Ok(res);
    })
        .Produces<Crop>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
}

// Routes per Attuatori e Sensori
void MapCropComponentsRoutes<T>(string routeName) where T : class, ICropComponent
{
    var fullRoute = $"{API_PREFIX}/farmingCompanies/{{companyId:int}}/crops/{{cropId:int}}/{routeName}";
    var group = app.MapGroup(fullRoute)
        .WithTags(routeName.Capitalize());
    
    // GET all
    group.MapGet("/",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager}")]
            ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId) =>
    {
        var res = repository.GetAll(companyId, cropId);
        return Results.Ok(res);
    })
        .Produces<IAsyncEnumerable<T>>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // GET single
    group.MapGet("/{id:int}",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager}")]
            async ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int id) =>
    {
        var res = await repository.Get(companyId, cropId, id);
        return Results.Ok(res);
    })
        .Produces<T>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // POST
    group.MapPost("/",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager}")]
            async ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromBody] T component) =>
    {
        var res = await repository.Add(companyId, cropId, component);
        return ResourceCreated(fullRoute, res);
    })
        .Produces<T>(201)
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // DELETE
    group.MapDelete("/{id:int}",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager}")]
            async ([FromServices] ICropComponentRepository<T> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int id) =>
    {
        var res = await repository.Delete(companyId, cropId, id);
        return Results.Ok(res);
    })
        .Produces<T>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
}

// Routes per Comandi degli Attuatori e Misurazioni dei Sensori
// TC sta per Type-Component, TCp sta per Type-ComponentProperty
void MapCropComponentsPropertiesRoutes<TC, TCp>(string routeName) where TC : class, IHasId, ICropComponent where TCp : class, IHasId, ICropComponentProperty
{
    var fullRoute = $"{API_PREFIX}/farmingCompanies/{{companyId:int}}/crops/{{cropId:int}}/{ typeof(TC).Name.UnCapitalize() }s/{{componentId:int}}/{ routeName }";
    var group = app.MapGroup(fullRoute)
        .WithTags(routeName.Capitalize());

    // GET all
    group.MapGet("/",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager},{Roles.IoTSubSystem}")]
            ([FromServices] ICropComponentPropertyRepository<TCp> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int componentId, [FromQuery] DateTime? between, [FromQuery] DateTime? and) =>
    {
        if (between != null)
        {
            return Results.Ok(repository.GetAll(companyId, cropId, componentId, between, and));
        }
        
        var res = repository.GetAll(companyId, cropId, componentId);
        return Results.Ok(res);
    })
        .Produces<IAsyncEnumerable<TCp>>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // GET single
    group.MapGet("/{id:int}",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager},{Roles.IoTSubSystem}")]
            async ([FromServices] ICropComponentPropertyRepository<TCp> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int componentId, [FromRoute] int id) =>
    {
        var res = await repository.Get(companyId, cropId, componentId, id);
        return Results.Ok(res);
    })
        .Produces<TCp>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // POST
    group.MapPost("/",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager},{Roles.IoTSubSystem}")]
            async ([FromServices] ICropComponentPropertyRepository<TCp> repository, [FromRoute] int companyId, [FromRoute] int cropId, [FromRoute] int componentId, [FromBody] TCp component) =>
    {
        var res = await repository.Add(companyId, cropId, componentId, component);
        return ResourceCreated(fullRoute, res);
    })
        .Produces<TCp>(201)
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
}

void MapWaterUsageRoutes()
{
    const string fullRoute = $"{ API_PREFIX }/farmingCompanies/{{companyId:int}}/waterUsages";
    var group = app.MapGroup(fullRoute)
        .WithTags("WaterUsages");
    
    // GET all
    group.MapGet("/", ([FromServices] IWaterUsageRepository repository, [FromRoute] int companyId, [FromQuery] DateTime? between, [FromQuery] DateTime? and) =>
    {
        if (between != null)
        {
            return Results.Ok(repository.GetAll(companyId, between, and));
        }
        
        var res = repository.GetAll(companyId);
        return Results.Ok(res);
    })
        .RequireAuthorization("AnyAuthUser")
        .Produces<IAsyncEnumerable<WaterUsage>>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);

    // GET single
    group.MapGet("/{id:int}", async ([FromServices] IWaterUsageRepository repository, [FromRoute] int companyId, [FromRoute] int id) =>
    {
        var crop = await repository.Get(companyId, id);
        return Results.Ok(crop);
    })
        .RequireAuthorization("AnyAuthUser")
        .Produces<WaterUsage>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // POST
    group.MapPost("/",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager}")]
            async ([FromServices] IWaterUsageRepository repository, [FromRoute] int companyId, [FromBody] WaterUsage usage) =>
    {
        var res = await repository.Add(companyId, usage);
        return ResourceCreated(fullRoute, res);
    })
        .Produces<WaterUsage>(201)
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
}

void MapReservationsRoutes()
{
    const string fullRoute = $"{ API_PREFIX }/reservations";
    var group = app.MapGroup(fullRoute)
        .WithTags("Reservations");
    
    // GET all
    group.MapGet("/", ([FromServices] IReservationRepository repository, int farmingCompanyId, int waterCompanyId, [FromQuery] DateTime? between, [FromQuery] DateTime? and) =>
    {
        if (between != null)
        {
            return Results.Ok(repository.GetAll(farmingCompanyId, waterCompanyId, between, and));
        }
        
        var res = repository.GetAll(farmingCompanyId, waterCompanyId);
        return Results.Ok(res);
    })
        .RequireAuthorization("AnyAuthUser")
        .Produces<IAsyncEnumerable<Reservation>>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // GET single
    group.MapGet("/{id:int}", async ([FromServices] IReservationRepository repository, int farmingCompanyId, int waterCompanyId, [FromRoute] int id) =>
    {
        var res = await repository.Get(farmingCompanyId, waterCompanyId, id);
        return Results.Ok(res);
    })
        .RequireAuthorization("AnyAuthUser")
        .Produces<Reservation>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // POST
    group.MapPost("/",
            [Authorize(Roles = $"{Roles.Admin},{Roles.FarmManager}")]       /* solo admin e FarmingCompany-Manager possono creare una nuova Prenotazione */
            async ([FromServices] IReservationRepository repository, int farmingCompanyId, int waterCompanyId, [FromBody] Reservation reservation) =>
    {
        var res = await repository.Add(farmingCompanyId, waterCompanyId, reservation);
        return ResourceCreated(fullRoute, res);
    })
        .Produces<Reservation>(201)
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // PUT
    group.MapPut("/{id:int}",
            [Authorize(Roles = $"{Roles.Admin},{Roles.WaterManager}")]      /* solo admin e WaterCompany-Manager possono aggiornare una Prenotazione, ad esempio per Confermarla o meno */
            async ([FromServices] IReservationRepository repository, int farmingCompanyId, int waterCompanyId, [FromRoute] int id, [FromBody] Reservation reservation) =>
    {
        var res = await repository.Update(farmingCompanyId, waterCompanyId, id, reservation);
        return Results.Ok(res);
    })
        .Produces<Reservation>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
}

void MapWaterLimitRoutes()
{
    const string fullRoute = $"{ API_PREFIX }/waterCompanies/{{companyId:int}}/limits";
    var group = app.MapGroup(fullRoute)
        .WithTags("WaterLimits");
    
    // GET all
    group.MapGet("/", ([FromServices] IWaterLimitRepository repository, [FromRoute] int companyId) =>
    {
        var res = repository.GetAll(companyId);
        return Results.Ok(res);
    })
        .RequireAuthorization("AnyAuthUser")
        .Produces<IAsyncEnumerable<WaterLimit>>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // GET single
    group.MapGet("/{farmingCompanyId:int}", async ([FromServices] IWaterLimitRepository repository, [FromRoute] int companyId, [FromRoute] int farmingCompanyId) =>
    {
        var res = await repository.Get(companyId, farmingCompanyId);
        return Results.Ok(res);
    })
        .RequireAuthorization("AnyAuthUser")
        .Produces<WaterLimit>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);

    // POST
    group.MapPost("/",
            [Authorize(Roles = $"{Roles.Admin},{Roles.WaterManager}")]
            async ([FromServices] IWaterLimitRepository repository, [FromRoute] int companyId, [FromQuery] int farmingCompanyId, [FromQuery] int limit) =>
    {
        var res = await repository.Add(companyId, farmingCompanyId, limit);
        return ResourceCreated(fullRoute, res);
    })
        .Produces<WaterLimit>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // PUT
    group.MapPut("/{id:int}",
            [Authorize(Roles = $"{Roles.Admin},{Roles.WaterManager}")]
            async ([FromServices] IWaterLimitRepository repository, [FromRoute] int companyId, [FromRoute] int id, [FromQuery] int newLimit) =>
    {
        var res = await repository.Update(id, newLimit);
        return Results.Ok(res);
    })
        .Produces<WaterLimit>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
    
    // DELETE
    group.MapDelete("/{id:int}",
            [Authorize(Roles = $"{Roles.Admin},{Roles.WaterManager}")]
            async ([FromServices] IWaterLimitRepository repository, [FromRoute] int companyId, [FromRoute] int id) =>
    {
        var res = await repository.Delete(id);
        return Results.Ok(res);
    })
        .Produces<WaterLimit>()
        .Produces<ErrorMessage>(401)
        .Produces<ErrorMessage>(403)
        .Produces<ErrorMessage>(404);
}

#endregion

IResult ResourceCreated<T>(string route, T resource) where T : IHasId => Results.Created($"{ route }/{ resource.Id }", resource);