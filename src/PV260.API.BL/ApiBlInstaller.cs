using Microsoft.Extensions.DependencyInjection;
using PV260.API.BL.Facades;
using PV260.API.BL.Services;
using PV260.Common;

namespace PV260.API.BL;

public class ApiBlInstaller : IInstaller
{
    public void Install(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ReportFacade>();
        serviceCollection.AddSingleton<IEmailService, SendgridEmailService>();
    }
}