using Microsoft.EntityFrameworkCore;
using PV260.API.DAL;

namespace PV260.API.IntegrationTests.Factories;

public class DbContextContainerFactory(string connectionString) : IDbContextFactory<MainDbContext>
{
    public MainDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<MainDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(Console.WriteLine);

        return new MainDbContext(optionsBuilder.Options);
    }
}