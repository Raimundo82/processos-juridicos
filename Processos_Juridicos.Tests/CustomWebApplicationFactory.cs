using System.Data.Common;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Moq;

using Processos_Juridicos.Data;
using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public Mock<IWindowsUserSvc> WindowsAuthSvcMock { get; }

    public CustomWebApplicationFactory()
    {
        WindowsAuthSvcMock = new Mock<IWindowsUserSvc>();
        WindowsAuthSvcMock
            .Setup(s => s.GetUserData())
            .Returns(new UserDataModel
            {
                DisplayName = "Mock User",
            });
        WindowsAuthSvcMock
            .Setup(s => s.GetUserGroups(It.IsAny<string>()))
            .Returns(["Admin", "User"]);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<DbConnection>();
            services.AddDbContextFactory<AppDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.RemoveAll<IWindowsUserSvc>();
            services.AddSingleton(WindowsAuthSvcMock.Object);
            services.PostConfigure<AuthenticationOptions>(options =>
            {
                if (options.SchemeMap.TryGetValue(NegotiateDefaults.AuthenticationScheme, out AuthenticationSchemeBuilder? negotiateScheme))
                {
                    negotiateScheme.HandlerType = typeof(TestAuthHandler);
                }
                options.DefaultScheme = NegotiateDefaults.AuthenticationScheme;
            });

            ServiceProvider sp = services.BuildServiceProvider();
            using IServiceScope scope = sp.CreateScope();
            IDbContextFactory<AppDbContext> dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using AppDbContext db = dbFactory.CreateDbContext();
            db.Database.EnsureCreated();
        });
    }
}
