using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PV260.API.DAL.Repositories;

namespace PV260.API.DAL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDalServices(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        serviceCollection.AddDbContext<MainDbContext>(options =>
            options.UseSqlServer(connectionString));

        serviceCollection.AddScoped<ReportRepository>();
        serviceCollection.AddScoped<EmailRepository>();
        
        return serviceCollection;
    }
}