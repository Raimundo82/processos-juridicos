using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Processos_Juridicos.Data;
using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.Tests;

public class CustomWebApplicationFactory<TProgram>(string databaseName) : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _databaseName = databaseName;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<DbConnection>();

            services.AddDbContextFactory<AppDbContext>(options => options.UseInMemoryDatabase(_databaseName));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using var db = dbFactory.CreateDbContext();
            db.Database.EnsureCreated();
        });
    }
}
