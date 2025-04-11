using Coravel;
using Microsoft.Extensions.Configuration;
using PV260.API.BL.Invocables;

namespace PV260.API.BL.Extensions;

/// <summary>
/// Provides extension methods for adding scheduled tasks to the service provider.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Adds scheduled tasks to the specified <see cref="IServiceProvider"/> using the provided configuration.
    /// </summary>
    /// <param name="serviceProvider">The service provider to which the scheduled tasks will be added.</param>
    /// <param name="configuration">The application configuration used to retrieve scheduling settings.</param>
    public static IServiceProvider AddScheduledTasks(this IServiceProvider serviceProvider, IConfiguration configuration)
    {
        // Retrieve the cron expression from the configuration
        // var cronExpression = configuration.GetValue<string>("ReportSettings:ScheduledGenerationCron");

        // serviceProvider.UseScheduler(scheduler =>
        // {
        //     // Automatic report generation
        //     scheduler.Schedule<GenerateReportInvocable>()
        //         .Cron(cronExpression)
        //         .PreventOverlapping("GenerateReport");
        //
        //     // Old report deletion
        //     scheduler.Schedule<DeleteOldReportsInvocable>()
        // });

        return serviceProvider;
    }
}