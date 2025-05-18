using Microsoft.Extensions.DependencyInjection;
using PV260.API.BL.Facades;
using PV260.API.Presentation.Facades;

namespace PV260.API.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection serviceCollection)
    {
        // Facades
        serviceCollection.AddSingleton<IReportFacade, ReportFacade>();
        serviceCollection.AddSingleton<IEmailFacade, EmailRecipientFacade>();

        return serviceCollection;
    }
}