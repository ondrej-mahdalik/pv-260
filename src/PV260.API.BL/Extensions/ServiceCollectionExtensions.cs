using Microsoft.Extensions.DependencyInjection;
using PV260.API.BL.Facades;

namespace PV260.API.BL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDBlServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ReportFacade>();
        
        return serviceCollection;
    }
}