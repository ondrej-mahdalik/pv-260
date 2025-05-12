using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;

namespace PV260.Client.BL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlServices(this IServiceCollection serviceCollection)
    {
        
        serviceCollection.AddResiliencePipeline(ApiClient.DefaultApiClientPipeline, builder =>
            builder
                .AddRetry(new RetryStrategyOptions
                    {
                        MaxRetryAttempts = 3, Delay = TimeSpan.FromMilliseconds(100)
                    })
                .AddTimeout(TimeSpan.FromSeconds(10)));
        
        serviceCollection.AddSingleton<IApiClient, ApiClient>();
        
        return serviceCollection;
    }
}