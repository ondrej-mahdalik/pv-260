using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content;
using PV260.Client.ConsoleApp.Components.Content.Emails;
using PV260.Client.ConsoleApp.Components.Content.Reports;
using PV260.Client.ConsoleApp.Components.Content.Reports.LatestGeneratedReport;
using PV260.Client.ConsoleApp.Components.Enums;
using PV260.Client.ConsoleApp.Components.Interfaces;

namespace PV260.Client.ConsoleApp.Components;

internal class DefaultContentRouter(IApiClient apiClient) : IContentRouter
{
    public IRenderableComponent Route(MenuItems menuItem)
    {
        return menuItem switch
        {
            MenuItems.Home => new HomeContentComponent(),
            MenuItems.LatestGeneratedReport => new LatestGeneratedReportComponent(apiClient),
            MenuItems.Reports => new ReportsOptionsComponent(),
            MenuItems.Emails => new EmailsOptionsComponent(),
            MenuItems.About => new AboutContentComponent(),
            _ => new HomeContentComponent()
        };
    }
}