using Microsoft.EntityFrameworkCore;

namespace PV260.API.DAL.Migrator;

public class DbMigrator(IDbContextFactory<MainDbContext> dbContextFactory) : IDbMigrator
{
    public void Migrate()
    {
        using var dbContext = dbContextFactory.CreateDbContext();
        dbContext.Database.Migrate();
    }
}