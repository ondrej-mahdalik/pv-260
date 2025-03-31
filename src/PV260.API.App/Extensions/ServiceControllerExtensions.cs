using PV260.Common;

namespace PV260.API.App.Extensions;

public static class ServiceControllerExtensions
{
    public static IServiceCollection AddInstaller<TInstaller>(this IServiceCollection serviceCollection)
        where TInstaller : class, IInstaller, new()
    {
        var installer = new TInstaller();
        installer.Install(serviceCollection);
        return serviceCollection;
    }
}