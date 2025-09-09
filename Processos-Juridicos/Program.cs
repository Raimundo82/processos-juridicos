using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;

using NToastNotify;

using Processos_Juridicos.Data;
using Processos_Juridicos.Middleware;
using Processos_Juridicos.Middleware.ExceptionHandlers;
using Processos_Juridicos.Services.Auth;
using Processos_Juridicos.Services.DomainData;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.Auth;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;
using Processos_Juridicos.Services.ProcessManagement;
using Processos_Juridicos.Services.UiHelpers;
using Processos_Juridicos.Utilities.TextManager;
using Processos_Juridicos.Utilities.TextManager.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Authentication SOO (via Negotiate)
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

// Set Authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;

    options.AddPolicy("OFICIAIS-INSTRUTORES", policy =>
        policy.RequireRole("OFICIAIS-INSTRUTORES"));

    options.AddPolicy("COMANDO-UNIDADE", policy =>
        policy.RequireRole("COMANDO-UNIDADE"));

    options.AddPolicy("DJ-PROCESSES", policy =>
        policy.RequireRole("DJ-AUTHORIZED", "DJ-UNAUTHORIZED", "SUPERADMIN"));

    options.AddPolicy("DJ-ADMINISTRATION", policy =>
        policy.RequireRole("DJ-AUTHORIZED", "SUPERADMIN"));

    options.AddPolicy("SUPER-ADMIN", policy =>
        policy.RequireRole("SUPERADMIN"));
});

// Enable session 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(3);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add IHttpContextAccessor so services and middleware can read HttpContext
builder.Services.AddHttpContextAccessor();

// Add User Secrets
builder.Configuration.AddUserSecrets<Program>();

// Register Context
var processosDj = builder.Configuration.GetConnectionString("ProcessosDJ_Dev")!;
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(processosDj).EnableSensitiveDataLogging().LogTo(Console.WriteLine, LogLevel.Debug));

builder.Services.AddSingleton<IJsonTextManager>(sp =>
{
    IWebHostEnvironment env = sp.GetRequiredService<IWebHostEnvironment>();
    var filePath = Path.Combine(env.ContentRootPath, "ResourceFiles", "systemtext.json");
    return new JsonTextManager(filePath);
});

//register Interfaces services
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
builder.Services.AddScoped<RoleSyncService>();
builder.Services.AddHostedService<TimedSyncService>();
builder.Services.AddScoped<ILegalReferenceSvc, LegalReferenceSvc>();
builder.Services.AddScoped<IContextSvc, ContextSvc>();
builder.Services.AddScoped<IProcessManagementSvc, ProcessManagementSvc>();
builder.Services.AddScoped<IProcessViewDataSvc, ProcessViewDataSvc>();
builder.Services.AddScoped<IFileValidatorSvc, FileValidatorSvc>();

// Interface service only supported on windows
if (OperatingSystem.IsWindows())
{
    builder.Services.AddScoped<NegotiateRoleMiddleware>();
    builder.Services.AddScoped<ILdapUserSvc, LdapUserSvc>();
}

//Register NToastNotify
builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
{
    ProgressBar = true,
    PositionClass = ToastPositions.TopCenter,
    TimeOut = 5000
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using IServiceScope scope = app.Services.CreateScope();
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (db.Database.IsRelational())
    {
        db.Database.Migrate();
    }
}

app.UseExceptionHandler("/Home/Error");

GlobalTextManager.SetManager(app.Services.GetRequiredService<IJsonTextManager>());


if (!app.Environment.IsDevelopment())
{


    AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
    {
        var ex = (Exception)eventArgs.ExceptionObject;
        Console.WriteLine("Unhandled exception: " + ex.ToString());
    };

    TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
    {
        // Log exception details
        Console.WriteLine("Unobserved task exception: " + eventArgs.Exception.ToString());
        eventArgs.SetObserved(); // prevents the process from terminating
    };
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();

if (OperatingSystem.IsWindows())
{
    app.UseMiddleware<NegotiateRoleMiddleware>();
}

app.UseMiddleware<SessionRoleMiddleware>();
app.UseAuthorization();
app.UseNToastNotify();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();

public partial class Program
{
    protected Program() { }
}
