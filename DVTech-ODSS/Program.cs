using DVTech_ODSS.Components;
using Microsoft.EntityFrameworkCore;
using DVTech_ODSS.Data;
using DVTech_ODSS.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MySQL Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Register Services
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<ProcurementService>();
builder.Services.AddScoped<ManpowerService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SalesService>();
builder.Services.AddScoped<PredictiveService>(); // NEW

// Register Authentication State Provider as Singleton
builder.Services.AddSingleton<AuthStateProvider>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Seed database with sample data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    await DbSeeder.SeedInventoryData(context);
    await DbSeeder.SeedDefaultUsers(context);

    // NEW: Seed 5 years of historical sales data
    await HistoricalDataSeeder.SeedHistoricalSalesData(context);
}

app.Run();