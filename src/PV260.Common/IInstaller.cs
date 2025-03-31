using Microsoft.Extensions.DependencyInjection;

namespace PV260.Common;

public interface IInstaller
{
    void Install(IServiceCollection serviceCollection);
}