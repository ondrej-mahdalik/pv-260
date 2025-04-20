using Coravel.Invocable;
using PV260.API.BL.Facades;

namespace PV260.API.BL.Invocables;

public class GenerateReportInvocable(ReportFacade reportFacade) : IInvocable
{
    private readonly ReportFacade _reportFacade = reportFacade;

    public async Task Invoke()
    {
        await _reportFacade.GenerateReportAsync();
    }
}