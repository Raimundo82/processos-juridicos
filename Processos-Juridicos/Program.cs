using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Processos_Juridicos.Data;
using Processos_Juridicos.Middleware.ExceptionHandlers;
using Processos_Juridicos.Services;
using Processos_Juridicos.Services.Interfaces;
using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Keycloak.AuthServices.Authorization;

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

// Keycloak

var host = builder.Host;
var configuration = builder.Configuration;
var services = builder.Services;

services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddKeycloakWebApp(
        builder.Configuration.GetSection(KeycloakAuthenticationOptions.Section),
        configureOpenIdConnectOptions: options =>
        {
            options.SaveTokens = true;
            options.BackchannelHttpHandler = new HttpClientHandler
            {
                UseProxy = false,
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,

            };
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.Events = new OpenIdConnectEvents
            {
                OnSignedOutCallbackRedirect = context =>
                {
                    context.Response.Redirect("/");
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        });


services.AddKeycloakAuthorization(configuration);

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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseNToastNotify();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/", () => "Hello World!").RequireAuthorization();

app.Run();
