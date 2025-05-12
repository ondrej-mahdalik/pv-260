using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;

namespace PV260.Client.BL.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the business logic services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="baseAddress">URL of the API service to be used by the client.</param>
    public static IServiceCollection AddBlServices(this IServiceCollection serviceCollection, string baseAddress)
    {
        serviceCollection.AddResiliencePipeline(ApiClient.DefaultApiClientPipeline, builder =>
            builder
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3, Delay = TimeSpan.FromMilliseconds(100)
                })
                .AddTimeout(TimeSpan.FromSeconds(10)));
        
        serviceCollection.AddHttpClient<IApiClient, ApiClient>(client => { client.BaseAddress = new Uri(baseAddress); });
        return serviceCollection;
    }
}