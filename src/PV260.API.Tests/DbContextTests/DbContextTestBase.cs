using PV260.API.DAL;
using PV260.API.DAL.Entities;
using PV260.API.Tests.Factories;
using PV260.API.Tests.Seeds;
using Testcontainers.MsSql;

namespace PV260.API.Tests.DbContextTests;

public abstract class DbContextTestBase : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
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
        await EmailEntitySeeds.SeedAsync(DbContextSut);
        await ReportEntitySeeds.SeedAsync(DbContextSut);
        await DbContextSut.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContextSut.DisposeAsync();
        await _container.DisposeAsync();
    }
}