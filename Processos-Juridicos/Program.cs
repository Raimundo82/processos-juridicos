using Keycloak.AuthServices.Authentication;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using NToastNotify;

using Processos_Juridicos.Data;
using Processos_Juridicos.Middleware;
using Processos_Juridicos.Middleware.ExceptionHandlers;
using Processos_Juridicos.Services.DomainData;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Services.Interfaces.Ldap;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;
using Processos_Juridicos.Services.Ldap;
using Processos_Juridicos.Services.ProcessManagement;
using Processos_Juridicos.Services.UiHelpers;
using Processos_Juridicos.Settings;
using Processos_Juridicos.Utilities;
using Processos_Juridicos.Utilities.TextManager;
using Processos_Juridicos.Utilities.TextManager.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

// Keycloak
ConfigurationManager configuration = builder.Configuration;
builder.Services.AddKeycloakAuthentication(configuration);


// Configure role-based authorization policies
AuthorizationBuilder authBuilder = builder.Services.AddAuthorizationBuilder();
authBuilder.AddCustomPolicies();
builder.Services.ConfigureFallbackPolicy();


// In-memory cache to support session state
builder.Services.AddDistributedMemoryCache();

// Session configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(3);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});


// Add services to the container - Enable login requirement functionality on all controllers
builder.Services.AddControllersWithViews(options =>
{
    AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.Filters.Add(new AuthorizeFilter(policy));
});

// Allows services to access HttpContext
builder.Services.AddHttpContextAccessor();

// Load development secrets
builder.Configuration.AddUserSecrets<Program>();

// Global Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Bind the "AppSettings" section from configuration to the AppSettingsOptions class
builder.Services.Configure<AppSettingsOptions>(builder.Configuration.GetSection(AppSettingsOptions.AppSettings));


// Connection string from configuration
var processosDj = builder.Configuration.GetConnectionString("DefaultConnection")!;

// Register DbContext with SQL Server and detailed logging
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(processosDj)
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Debug));


// JSON text manager for system text (systemtext.json)
builder.Services.AddSingleton<IJsonTextManager>(sp =>
{
    IWebHostEnvironment env = sp.GetRequiredService<IWebHostEnvironment>();
    var filePath = Path.Combine(env.ContentRootPath, "ResourceFiles", "systemtext.json");
    return new JsonTextManager(filePath);
});


// Register Interfaces services
builder.Services.AddScoped<IToastNotify, ToastNotify>();
builder.Services.AddScoped<IUnitSvc, UnitSvc>();
builder.Services.AddScoped<IProcessStateSvc, StateSvc>();
builder.Services.AddScoped<ISentenceSvc, SentenceSvc>();
builder.Services.AddScoped<IProcessTypeSvc, ProcessTypesSvc>();
builder.Services.AddScoped<IHarmedOrCasualtySvc, HarmedOrCasualtySvc>();
builder.Services.AddScoped<IInfringementSvc, InfringementSvc>();
builder.Services.AddScoped<IProcessFileSvc, ProcessFileSvc>();
builder.Services.AddScoped<IProcessSvc, ProcessSvc>();
builder.Services.AddScoped<IAccidentTypeSvc, AccidentTypeSvc>();
builder.Services.AddScoped<IStateTransitionSvc, StateTransitionSvc>();
builder.Services.AddScoped<ICrimeTypeSvc, CrimeTypeSvc>();
builder.Services.AddScoped<IMilitarySecuritySvc, MilitarySecuritySvc>();
builder.Services.AddScoped<IUserSvc, UserSvc>();
builder.Services.AddScoped<IRoleSvc, RoleSvc>();
//builder.Services.AddScoped<RoleSyncSvc>();
//builder.Services.AddHostedService<TimedSyncSvc>();
builder.Services.AddScoped<ILegalReferenceSvc, LegalReferenceSvc>();
builder.Services.AddScoped<IContextSvc, ContextSvc>();
builder.Services.AddScoped<IProcessManagementSvc, ProcessManagementSvc>();
builder.Services.AddScoped<IProcessViewDataSvc, ProcessViewDataSvc>();
builder.Services.AddScoped<IFileValidatorSvc, FileValidatorSvc>();

// Interface service only supported on windows
if (OperatingSystem.IsWindows())
{
    builder.Services.AddScoped<ILdapUserSvc, LdapUserSvc>();
}


// Register NToastNotify (Notifications)
builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
{
    ProgressBar = true,
    PositionClass = ToastPositions.TopCenter,
    TimeOut = 5000
});


WebApplication app = builder.Build();

// Automatic Migrations (Development Only)
if (app.Environment.IsDevelopment())
{
    using IServiceScope scope = app.Services.CreateScope();
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (db.Database.IsRelational())
    {
        db.Database.Migrate();
    }
}


// Error & Exception Configuration
app.UseExceptionHandler("/Home/Error");

// Set the global text manager
GlobalTextManager.SetManager(app.Services.GetRequiredService<IJsonTextManager>());


// Global unhandled exception logging (Production only)
if (!app.Environment.IsDevelopment())
{

    AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
    {
        var ex = (Exception)eventArgs.ExceptionObject;
        Console.WriteLine("Unhandled exception: " + ex.ToString());
    };

    TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
    {
        Console.WriteLine("Unobserved task exception: " + eventArgs.Exception.ToString());
        eventArgs.SetObserved();
    };
}


// Get AppSettingsOptions values from configuration via dependency injection
AppSettingsOptions appSettings = app.Services.GetRequiredService<IOptions<AppSettingsOptions>>().Value;

// HTTP Pipeline
app.UsePathBase(appSettings.SubPath);
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<SessionRoleMiddleware>();
app.UseAuthorization();
app.UseNToastNotify();

// Default route mapping
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();

public partial class Program
{
    protected Program() { }
}
