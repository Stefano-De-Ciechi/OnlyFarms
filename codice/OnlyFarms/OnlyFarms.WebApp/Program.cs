using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
InjectRepositories(builder.Services, builder.Configuration);

// Add services to the container.
/*var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
*/

/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DataContext>();*/

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddIdentityCore<ApplicationUser>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

builder.Services.AddRazorPages();

// aggiunta dell'autenticazione con Oauth2 (provider Google)    --> installato il pacchetto NuGet "Microsof.AspNetCore.Authentication.Google"
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    // builder.Configuration accede automaticamente a tutti i file di configurazione (anche gli user-secrets) e li rende accessibili come se fosse una hash table
    var configuration = builder.Configuration;
    
    /* questi due dati sono salvati nei .NET user-secrets (file di configurazione "nascosto" che solitamente non viene tracciato nei Version Control Systems tipo git)
        - in Rider tasto destro sul progetto --> Tools --> .NET user secrets
        - in Visual Studio tasto destro sul progetto --> manage user secrets
        - da cli --> dotnet user secrets init --> dotnet user-secrets set "<key>" "<value>" --> oppure un modo piu' veloce cat ./input.json | dotnet user-secrets set in cui in input.json si inseriscono tutti i segreti in una volta sola (ricordarsi di cancellare il file)

    per generarli: https://learn.microsoft.com/it-it/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-7.0
    */
    googleOptions.ClientId = configuration["Authentication:Google:ClientId"]!;
    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Roles.Admin, policy =>
        //policy.RequireRole(Roles.Admin)
        policy.RequireClaim(nameof(Roles), Roles.Admin)     // TODO capire quale delle due versioni sia corretta
    );
    
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

// Add any data seeding logic here

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

await AddAdmin();

app.Run();

return;

TService GetService<TService>(IServiceProvider services) where TService : notnull
{
    return services.GetRequiredService<TService>();
}

void InjectRepositories(IServiceCollection services, IConfiguration configuration)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    /*var options = new DbContextOptionsBuilder<DataContext>()       // DataContext e' definita in OnlyFarms.Core
        .UseSqlite(connectionString, b => b.MigrationsAssembly("OnlyFarms.RestApi"))
        .Options;*/
    
    builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    //dataContext.Database.EnsureCreated();     // rimosso perche' ri-creava il DB ogni volta che si provava ad eseguire una migrazione, comportando poi degli errori (Tabelle gia' esistenti) quando si eseguiva il comando "database update"
    
    /*Questa dichiarazione di injection è stata fatta perchè noi dichiaravano dataContext come var creando un conflitto con addDbContext, facendo cosi avveniva
     * un'implementazione manuale che non è molto utile in quanto viene fatto tutto in automatio creando quindi un problema.
    */
    services.AddScoped<ICompanyRepository<FarmingCompany>>(serviceProvider=> new CompanyRepository<FarmingCompany>(GetService<DataContext>(serviceProvider)));
    services.AddScoped<ICompanyRepository<WaterCompany>>(serviceProvider => new CompanyRepository<WaterCompany>(GetService<DataContext>(serviceProvider)));
    services.AddScoped<ICropRepository>(serviceProvider => new CropRepository(GetService<DataContext>(serviceProvider), GetService<ICompanyRepository<FarmingCompany>>(serviceProvider)));
    services.AddScoped<ICropComponentRepository<Actuator>>(serviceProvider => new CropComponentRepository<Actuator>(GetService<DataContext>(serviceProvider), GetService<ICropRepository>(serviceProvider)));
    services.AddScoped<ICropComponentRepository<Sensor>>(serviceProvider => new CropComponentRepository<Sensor>(GetService<DataContext>(serviceProvider), GetService<ICropRepository>(serviceProvider)));
    services.AddScoped<ICropComponentPropertyRepository<Command>>(serviceProvider => new CropComponentPropertyRepository<Actuator, Command>(GetService<DataContext>(serviceProvider), GetService<ICropComponentRepository<Actuator>>(serviceProvider)));
    services.AddScoped<ICropComponentPropertyRepository<Measurement>>(serviceProvider => new CropComponentPropertyRepository<Sensor, Measurement>(GetService<DataContext>(serviceProvider), GetService<ICropComponentRepository<Sensor>>(serviceProvider)));
    services.AddScoped<IWaterUsageRepository>(serviceProvider => new WaterUsageRepository(GetService<DataContext>(serviceProvider), GetService<ICompanyRepository<FarmingCompany>>(serviceProvider)));
    services.AddScoped<IReservationRepository>(serviceProvider => new ReservationRepository(GetService<DataContext>(serviceProvider), GetService<ICompanyRepository<FarmingCompany>>(serviceProvider), GetService<ICompanyRepository<WaterCompany>>(serviceProvider)));
    services.AddScoped<IWaterLimitRepository>(serviceProvider => new WaterLimitRepository(GetService<DataContext>(serviceProvider), GetService<ICompanyRepository<FarmingCompany>>(serviceProvider), GetService<ICompanyRepository<WaterCompany>>(serviceProvider)));
}

async Task AddAdmin()
{
    var services = app!.Services!.CreateScope().ServiceProvider;            // si crea uno scope, visto che in questo punto del programma l'applicazione non e' ancora stata avviata

    var adminUser = new User();     // crea un oggetto che contiene due campi di tipo stringa
    app.Configuration.Bind("Admin", adminUser);     // legge dal file di configurazione (in questo caso user secrets) l'oggetto "Admin" : { "UserName" : "...", "Password" : "..." } e salva i dati letti nell'oggetto adminUser

    //var usersManager = services.GetRequiredService<UserManager<IdentityUser>>();         // questa e' una classe creata in automatico da asp.net per gestire gli utenti (probabilmente accede al DB); viene "presa" dal contenitore delle dipendenze; UserManager viene iniettato nelle dipendenze dal metodo builder.Services.AddAuthentication()
    var usersManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    
    //var user = new IdentityUser()
    var user = new ApplicationUser()
    {
        UserName = adminUser.UserName!,
        EmailConfirmed = true,      // aggiunta altrimenti il login NON funzionava (invalid login credentials error)
    };               // crea un istanza di IdentityUser (il tipo di utente salvato nel DB) passandogli l'username letto dagli user serets
    
    if ((await usersManager.CreateAsync(user, adminUser.Password!)).Succeeded)       // CreateAsync si aspetta in input un'istanza di IdentityUser e una stringa come password
    {
        await usersManager.AddClaimAsync(user, new Claim(nameof(Roles), Roles.Admin));      // Role e' una classe statica "di convenienza" definita in Data/Role.cs
    }
}

// TODO spostare da qualche parte; magari bisogna aggiungere dei campi aggiuntivi, es. a quale azienda (e quale tipo di azienda) fa parte (gli admin NON hanno azienda)
internal record User
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
}