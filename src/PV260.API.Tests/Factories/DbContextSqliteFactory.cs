using Microsoft.EntityFrameworkCore;
using PV260.API.DAL;

namespace PV260.API.Tests.Factories;

public class DbContextSqliteFactory : IDbContextFactory<MainDbContext>
{
    private readonly DbContextOptionsBuilder<MainDbContext> _contextOptionsBuilder = new();

    public DbContextSqliteFactory()
    {
        _contextOptionsBuilder.UseSqlite("DataSource=TestData.db.temp");
        _contextOptionsBuilder.EnableSensitiveDataLogging();
        _contextOptionsBuilder.LogTo(Console.WriteLine);
    }
    
    public MainDbContext CreateDbContext()
    {
        return new MainDbContext(_contextOptionsBuilder.Options);
    }
}