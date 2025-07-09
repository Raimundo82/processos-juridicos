using System.Data.Common;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Processos_Juridicos.Data;

namespace Processos_Juridicos.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<DbConnection>();

            services.AddDbContextFactory<AppDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            ServiceProvider sp = services.BuildServiceProvider();
            using IServiceScope scope = sp.CreateScope();
            IDbContextFactory<AppDbContext> dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
            using AppDbContext db = dbFactory.CreateDbContext();
            db.Database.EnsureCreated();
        });
    }
}
