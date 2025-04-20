using Coravel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PV260.API.BL.Invocables;
using PV260.API.BL.Options;

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
    public static IServiceProvider AddScheduledTasks(this IServiceProvider serviceProvider)
    {
        var settings = serviceProvider.GetRequiredService<IOptions<ReportOptions>>();

        serviceProvider.UseScheduler(scheduler =>
        {
            // Automatic report generation
            scheduler.Schedule<GenerateReportInvocable>()
                .Cron(settings.Value.ReportGenerationCron)
                .PreventOverlapping(nameof(GenerateReportInvocable));
        
            // Old report deletion
            scheduler.Schedule<DeleteOldReportsInvocable>()
                .Cron(settings.Value.ReportGenerationCron)
                .PreventOverlapping(nameof(DeleteOldReportsInvocable));
        });

        return serviceProvider;
    }
}