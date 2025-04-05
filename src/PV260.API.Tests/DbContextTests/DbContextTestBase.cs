using PV260.API.DAL;
using PV260.API.Tests.Factories;

namespace PV260.API.Tests.DbContextTests;

public abstract class DbContextTestBase : IAsyncLifetime
{
    protected readonly DbContextSqliteFactory DbContextFactory = new();

    protected MainDbContext DbContextSut { get; }

    protected DbContextTestBase()
    {
        DbContextSut = DbContextFactory.CreateDbContext();
    }
    
    public async Task InitializeAsync()
    {
        await DbContextSut.Database.EnsureDeletedAsync();
        await DbContextSut.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContextSut.DisposeAsync();
    }
}