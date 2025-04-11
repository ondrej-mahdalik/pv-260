using Microsoft.EntityFrameworkCore;
using PV260.API.DAL;
using PV260.API.DAL.UnitOfWork;
using PV260.API.Tests.Factories;

namespace PV260.API.Tests;

public class DalTestBase : IAsyncLifetime
{
    protected readonly DbContextSqliteFactory DbContextFactory = new();

    protected MainDbContext DbContextSUT { get; }

    protected DalTestBase()
    {
        DbContextSUT = DbContextFactory.CreateDbContext();
    }
    
    public async Task InitializeAsync()
    {
        await DbContextSUT.Database.EnsureDeletedAsync();
        await DbContextSUT.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContextSUT.Database.EnsureDeletedAsync();
        await DbContextSUT.DisposeAsync();
    }
}