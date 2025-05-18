using Coravel;
using Microsoft.Extensions.DependencyInjection;
using PV260.API.BL.Services;
using PV260.API.Infrastructure.Invocables;
using PV260.API.Infrastructure.Services;

namespace PV260.API.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection serviceCollection)
    {
        // Services
        serviceCollection.AddSingleton<IEmailService, SendgridEmailService>();
        
        // Invocables
        serviceCollection.AddTransient<GenerateReportInvocable>();
        serviceCollection.AddTransient<DeleteOldReportsInvocable>();
        
        // Scheduling
        serviceCollection.AddScheduler();
        
        return serviceCollection;
    }
}