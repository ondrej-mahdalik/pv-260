using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using PV260.API.BL.Facades;

namespace PV260.API.Infrastructure.Invocables;

public class DeleteOldReportsInvocable(IReportFacade reportFacade, ILogger<DeleteOldReportsInvocable> logger) : IInvocable
{
    public async Task Invoke()
    {
        logger.LogDebug($"Scheduled report cleanup has been triggered.");
        var reportsDeleted = await reportFacade.DeleteOldReportsAsync();
        
        logger.LogDebug("Scheduled report cleanup has been successfully completed. Deleted reports count: {DeletedReportsCount}", reportsDeleted);
    }
}