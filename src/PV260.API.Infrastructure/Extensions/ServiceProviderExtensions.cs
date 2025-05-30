﻿using Coravel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PV260.API.BL;
using PV260.API.BL.Options;
using PV260.API.Infrastructure.Invocables;

namespace PV260.API.Infrastructure.Extensions;

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
        var logger = serviceProvider.GetRequiredService<ILogger<BusinessLogic>>();
        var settings = serviceProvider.GetRequiredService<IOptions<ReportOptions>>().Value;

        serviceProvider.UseScheduler(scheduler =>
        {
            // Automatic report generation
            scheduler.Schedule<GenerateReportInvocable>()
                .Cron(settings.ReportGenerationCron)
                .PreventOverlapping(nameof(GenerateReportInvocable));

            // Old report deletion
            scheduler.Schedule<DeleteOldReportsInvocable>()
                .Cron(settings.OldReportCleanupCron)
                .PreventOverlapping(nameof(DeleteOldReportsInvocable));
        }).OnError(exception =>
        {
            logger.LogError(exception, "An exception has occured while running a scheduled task.");
        });

        return serviceProvider;
    }
}