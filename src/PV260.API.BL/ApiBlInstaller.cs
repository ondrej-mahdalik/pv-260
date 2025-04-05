using Microsoft.Extensions.DependencyInjection;
using PV260.API.BL.Facades;

namespace PV260.API.BL;

public class ApiBlInstaller
{
    public void Install(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ReportFacade>();
    }
}