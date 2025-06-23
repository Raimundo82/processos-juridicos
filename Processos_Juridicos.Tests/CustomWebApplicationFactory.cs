using System.Data.Common;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Processos_Juridicos.Data;

namespace Processos_Juridicos.Tests;

public class CustomWebApplicationFactory<TProgram>(string databaseName) : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _databaseName = databaseName;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder.ConfigureServices(services =>
        {
            _ = services.RemoveAll<DbContextOptions<AppDbContext>>();
            _ = services.RemoveAll<DbConnection>();

            _ = services.AddDbContextFactory<AppDbContext>(options => options.UseInMemoryDatabase(_databaseName));

            ServiceProvider sp = services.BuildServiceProvider();
            using IServiceScope scope = sp.CreateScope();
            IDbContextFactory<AppDbContext> dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using AppDbContext db = dbFactory.CreateDbContext();
            _ = db.Database.EnsureCreated();
        });
    }
}
