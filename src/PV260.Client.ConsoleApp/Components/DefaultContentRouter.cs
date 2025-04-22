using PV260.Client.ConsoleApp.Components.Content;
using PV260.Client.ConsoleApp.Components.Content.Emails;
using PV260.Client.ConsoleApp.Components.Content.Reports;
using PV260.Client.ConsoleApp.Components.Enums;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.BL;

namespace PV260.Client.ConsoleApp.Components;

internal class DefaultContentRouter(IApiClient apiClient) : IContentRouter
{
    private readonly IApiClient _apiClient = apiClient;

    public IRenderableComponent Route(MenuItems menuItem)
    {
        return menuItem switch
        {
            MenuItems.Home => new HomeContentComponent(),
            MenuItems.Reports => new ReportsOptionsComponent(),
            MenuItems.Emails => new EmailsOptionsComponent(_apiClient),
            MenuItems.About => new AboutContentComponent(),
            _ => new HomeContentComponent()
        };
    }
}
