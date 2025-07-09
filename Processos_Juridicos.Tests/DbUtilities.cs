using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
namespace Processos_Juridicos.Tests;

public static class DbUtilities
{
    public static async Task RemoveEntitiesAsync<TEntity>(AppDbContext dbContext) where TEntity : class
    {
        List<TEntity> entities = await dbContext.Set<TEntity>().ToListAsync();
        dbContext.RemoveRange(entities);
        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();
        Assert.Empty(await dbContext.Set<TEntity>().ToListAsync());
    }
}
