using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PV260.API.DAL.Migrator;
using PV260.API.DAL.UnitOfWork;

namespace PV260.API.DAL.Extensions;

/// <summary>
/// Provides extension methods for registering data access layer (DAL) services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds data access layer services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection">The service collection to which the services will be added.</param>
    /// <param name="configuration">The application configuration used to retrieve the connection string.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with the registered services.</returns>
    public static IServiceCollection AddDalServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        serviceCollection.AddDbContextFactory<MainDbContext>(options =>
            options.UseSqlServer(connectionString));

        var recreateDatabase = configuration["RecreateDatabase"] == "true";
        if (recreateDatabase)
            serviceCollection.AddSingleton<IDbMigrator, CleanDbMigrator>();
        else
            serviceCollection.AddSingleton<IDbMigrator, DbMigrator>();
        
        serviceCollection.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

        return serviceCollection;
    }
}