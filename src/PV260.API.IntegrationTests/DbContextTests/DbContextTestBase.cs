using PV260.API.DAL;
using PV260.API.IntegrationTests.Factories;
using PV260.API.IntegrationTests.Seeds;
using Testcontainers.MsSql;

namespace PV260.API.IntegrationTests.DbContextTests;

public abstract class DbContextTestBase : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-CU18-ubuntu-22.04")
        .Build();

    protected DbContextContainerFactory DbContextFactory { get; private set; } = null!;

    protected MainDbContext DbContextSut { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // Start the DB container
        await _container.StartAsync();

        // Setup database
        DbContextFactory = new DbContextContainerFactory(_container.GetConnectionString());
        DbContextSut = DbContextFactory.CreateDbContext();

        await DbContextSut.Database.EnsureCreatedAsync();
        
        // Seed data
        await using var dbContext = DbContextFactory.CreateDbContext();
        await EmailEntitySeeds.SeedAsync(dbContext);
        await ReportEntitySeeds.SeedAsync(dbContext);
        await ReportRecordEntitySeeds.SeedAsync(dbContext);
        await dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContextSut.DisposeAsync();
        await _container.DisposeAsync();
    }
}