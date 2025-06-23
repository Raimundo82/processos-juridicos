using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Processos_Juridicos.Data;
using Processos_Juridicos.Middleware.ExceptionHandlers;
using Processos_Juridicos.Services;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.TextManager;
using Processos_Juridicos.Utilities.TextManager.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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
    app.UseDeveloperExceptionPage();
    app.UseHsts();

    AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
    {
        var ex = (Exception)eventArgs.ExceptionObject;
        // Log or write the exception details to a persistent log
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
app.UseRouting();
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