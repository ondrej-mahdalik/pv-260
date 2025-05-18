using Coravel;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
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
        serviceCollection.AddHttpClient<IReportService, ArkFundsReportService>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(500, retryAttempt))));
        
        // Invocables
        serviceCollection.AddTransient<GenerateReportInvocable>();
        serviceCollection.AddTransient<DeleteOldReportsInvocable>();
        
        // Scheduling
        serviceCollection.AddScheduler();
        
        return serviceCollection;
    }
}