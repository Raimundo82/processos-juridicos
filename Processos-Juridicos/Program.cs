using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Processos_Juridicos.Data;
using Processos_Juridicos.Middleware.ExceptionHandlers;
using Processos_Juridicos.Services;
using Processos_Juridicos.Services.Interfaces;
using Keycloak.AuthServices.Authentication;
using Processos_Juridicos.Middleware;
using Processos_Juridicos.Utilities.TextManager;
using Processos_Juridicos.Utilities.TextManager.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container - Enable login requirement functionality on all controllers
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Add User Secrets
builder.Configuration.AddUserSecrets<Program>();

// Register Context
string processosDj = builder.Configuration.GetConnectionString("processosDj")!;
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(processosDj));

//httpClient
builder.Services.AddHttpClient<ApisSvc>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseProxy = false,
        //ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
    });

// Keycloak
var configuration = builder.Configuration;
var services = builder.Services;

services.AddKeycloakAuthentication(configuration);

builder.Services.AddSingleton<IJsonTextManager>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    string filePath = Path.Combine(env.ContentRootPath, "ResourceFiles", "systemtext.json");
    return new JsonTextManager(filePath);
});

//register Interfaces services
builder.Services.AddScoped<IToastNotify, ToastNotify>();
builder.Services.AddScoped<IUnitSvc, UnitSvc>();
builder.Services.AddScoped<ISectorSvc, SectorSvc>();
builder.Services.AddScoped<IStateSvc, StateSvc>();
builder.Services.AddScoped<ISentenceSvc, SentenceSvc>();
builder.Services.AddScoped<IProcessTypeSvc, ProcessTypesSvc>();
builder.Services.AddScoped<IApisSvc, ApisSvc>();
builder.Services.AddScoped<IHarmedOrCasualtySvc, HarmedOrCasualtySvc>();
builder.Services.AddScoped<IInfringementSvc, InfringementSvc>();
builder.Services.AddScoped<ISectorSvc, SectorSvc>();
builder.Services.AddScoped<IProcessFileSvc, ProcessFileSvc>();
builder.Services.AddScoped<IProcessSvc, ProcessSvc>();
builder.Services.AddScoped<IAccidentTypeSvc, AccidentTypeSvc>();
builder.Services.AddScoped<ICrimeTypeSvc, CrimeTypeSvc>();
builder.Services.AddScoped<IMilitarySecuritySvc, MilitarySecuritySvc>();

//Register NToastNotify
builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
{
    ProgressBar = true,
    PositionClass = ToastPositions.TopCenter,
    TimeOut = 5000
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();
app.UseExceptionHandler("/Home/Error");

GlobalTextManager.SetManager(app.Services.GetRequiredService<IJsonTextManager>());

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseKeycloak();

app.UseAuthentication();
app.UseAuthorization();
app.UseNToastNotify();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
