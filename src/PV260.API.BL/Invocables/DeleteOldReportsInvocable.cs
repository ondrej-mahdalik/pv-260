using Coravel.Invocable;
using PV260.API.BL.Facades;

namespace PV260.API.BL.Invocables;

public class DeleteOldReportsInvocable(ReportFacade reportFacade) : IInvocable
{
    private readonly ReportFacade _reportFacade = reportFacade;

    public Task Invoke()
    {
        // TODO Call the report facade to delete old reports here once implemented
        throw new NotImplementedException();
    }
}