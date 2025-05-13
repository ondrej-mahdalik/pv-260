using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using PV260.API.BL.Facades;

namespace PV260.API.Infrastructure.Invocables;

/// <summary>
/// Represents a scheduled task for generating reports.
/// </summary>
/// <param name="reportFacade">The facade responsible for report generation.</param>
/// <param name="logger">The logger used for logging information and debug messages.</param>
public class GenerateReportInvocable(IReportFacade reportFacade, ILogger<GenerateReportInvocable> logger) : IInvocable
{
    public async Task Invoke()
    {
        logger.LogDebug($"Scheduled report generation has been triggered.");
        var report = await reportFacade.GenerateReportAsync();

        logger.LogDebug("Scheduled report has been successfully generated. Report ID: {ReportId}", report.Id);
    }
}