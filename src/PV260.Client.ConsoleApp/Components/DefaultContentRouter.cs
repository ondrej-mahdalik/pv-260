using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content;
using PV260.Client.ConsoleApp.Components.Content.Emails;
using PV260.Client.ConsoleApp.Components.Content.Reports;
using PV260.Client.ConsoleApp.Components.Enums;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;

namespace PV260.Client.ConsoleApp.Components;
internal class DefaultContentRouter : IContentRouter
{
    private readonly IApiClient _apiClient;
    private readonly INavigationService _navigationService;

    public DefaultContentRouter(IApiClient apiClient, INavigationService navigationService)
    {
        _apiClient = apiClient;
        _navigationService = navigationService;
    }

    public IRenderableComponent Route(MenuItems menuItem)
    {
        return menuItem switch
        {
            MenuItems.Home => new HomeContentComponent(),
            MenuItems.Reports => new ReportsOptionsComponent(_apiClient, _navigationService),
            MenuItems.Emails => new EmailsOptionsComponent(_apiClient),
            MenuItems.About => new AboutContentComponent(),
            _ => new HomeContentComponent()
        };
    }
}
