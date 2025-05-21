using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Processos_Juridicos.Data;
using Processos_Juridicos.Services;
using Processos_Juridicos.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add User Secrets
builder.Configuration.AddUserSecrets<Program>();

// Register Context
string processosDj = builder.Configuration.GetConnectionString("processosDj")!;
builder.Services.AddDbContext<AppDbContext>(opt=>opt.UseSqlServer(processosDj));


//httpClient
builder.Services.AddHttpClient<ApisSvc>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
         UseProxy = false,
        //ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true


    });

//register Interfaces services
builder.Services.AddScoped<IToastNotify, ToastNotify>();
builder.Services.AddScoped<IUnitSvc, UnitSvc>();
builder.Services.AddScoped<IStateSvc, StateSvc>();
builder.Services.AddScoped<IProcessTypesSvc, ProcessTypesSvc>();
builder.Services.AddScoped<ISentencesSvc, SentencesSvc>();
builder.Services.AddScoped<IApisSvc, ApisSvc>();
builder.Services.AddScoped<IHarmedOrCasualtiesSvc, HarmedOrCasualtiesSvc>();


//Register NToastNotify
builder.Services.AddMvc().AddNToastNotifyToastr(new ToastrOptions()
{
    ProgressBar = true,
    PositionClass = ToastPositions.TopCenter,
    TimeOut = 5000
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseNToastNotify();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
