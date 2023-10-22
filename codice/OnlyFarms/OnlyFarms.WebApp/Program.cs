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
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DataContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

builder.Services.AddRazorPages();

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

app.UseAuthorization();

app.MapRazorPages();

app.Run();

return;

TService GetService<TService>(IServiceProvider services) where TService : notnull
{
    return services.GetRequiredService<TService>();
}

void InjectRepositories(IServiceCollection services, IConfiguration configuration)
{
    //var connectionString = configuration.GetConnectionString("(default)");
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    var options = new DbContextOptionsBuilder<DataContext>()       // DataContext e' definita in OnlyFarms.Core
        .UseSqlite(connectionString, b => b.MigrationsAssembly("OnlyFarms.RestApi"))    // TODO quando si aggiungera' la parte OnlyFarms.WebApp, e' piu' logico spostare il database e le migrazioni in quel progetto!
        .Options;
    var dataContext = builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(connectionString));
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    //dataContext.Database.EnsureCreated();     // rimosso perche' ri-creava il DB ogni volta che si provava ad eseguire una migrazione, comportando poi degli errori (Tabelle gia' esistenti) quando si eseguiva il comando "database update"
    
    //services.AddScoped<ICompanyRepository<Reservation>>(_ => new CompanyRepository<Reservation>(dataContext));
    
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