using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
InjectRepositories(builder.Services, builder.Configuration);

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
        policy.RequireClaim(nameof(Roles), Roles.Admin)
    );
    
    options.AddPolicy(Roles.FarmManager, policy =>
    {
        policy.RequireClaim(nameof(Roles), Roles.FarmManager);
    });
    
    options.AddPolicy(Roles.WaterManager, policy =>
    {
        policy.RequireClaim(nameof(Roles), Roles.WaterManager);
    });
    
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
    
    builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    //dataContext.Database.EnsureCreated();     // rimosso perche' ri-creava il DB ogni volta che si provava ad eseguire una migrazione, comportando poi degli errori (Tabelle gia' esistenti) quando si eseguiva il comando "database update"
    
    
    /* Usato questo tipo di dichiarazione di dependency injection (in cui DataContext e' preso direttamente dai servizi) perche' provando a creare una variabile di
       tipo DataContext da passare alle repository si creava un conflitto con il metodo AddDbContext */
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

    services.AddScoped<ITokenRepository, TokenRepository>();
}

async Task AddAdmin()
{
    var services = app!.Services!.CreateScope().ServiceProvider;            // si crea uno scope, visto che in questo punto del programma l'applicazione non e' ancora stata avviata

    var adminUser = new User();     // crea un oggetto che contiene due campi di tipo stringa (definito sotto)
    app.Configuration.Bind("Admin", adminUser);     // legge dal file di configurazione (in questo caso user secrets) l'oggetto "Admin" : { "UserName" : "...", "Password" : "..." } e salva i dati letti nell'oggetto adminUser
    
    var usersManager = services.GetRequiredService<UserManager<ApplicationUser>>();     // UserManager e' una classe creata in automatico da asp.net per gestire gli utenti e viene "presa" dal contenitore delle dipendenze (UserManager viene iniettato nelle dipendenze dal metodo builder.Services.AddAuthentication())
    
    var user = new ApplicationUser()
    {
        UserName = adminUser.UserName!,
        EmailConfirmed = true,  // nella parte di autorizzazione il flag RequireConfirmedAccount e' settato a true (per poter creare il meccanismo di conferma di un account da parte degli utenti admin)
    };  // crea un istanza di ApplicationUser (il tipo di utente salvato nel DB) passandogli le credenzali lette dagli user serets
    
    if ((await usersManager.CreateAsync(user, adminUser.Password!)).Succeeded)       // CreateAsync si aspetta in input un'istanza di IdentityUser e una stringa come password
    {
        await usersManager.AddClaimAsync(user, new Claim(nameof(Roles), Roles.Admin));      // Role e' una classe statica "di convenienza" definita in Data/Role.cs
    }
}

// record usato solamente per raccogliere le informazioni sull'utente admin dal file user-secrets, non viene piu usato nel resto del progetto
internal record User
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
}