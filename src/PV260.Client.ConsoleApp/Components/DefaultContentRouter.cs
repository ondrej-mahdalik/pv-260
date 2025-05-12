using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content;
using PV260.Client.ConsoleApp.Components.Content.Emails.EmailOptions;
using PV260.Client.ConsoleApp.Components.Content.Reports.ReportDetail;
using PV260.Client.ConsoleApp.Components.Content.Reports.ReportOptions;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Layout.Enums;
using PV260.Client.ConsoleApp.Components.Layout.Interfaces;

namespace PV260.Client.ConsoleApp.Components;

internal class DefaultContentRouter(IApiClient apiClient) : IContentRouter
{
    private readonly IApiClient _apiClient = apiClient;

    public IRenderableComponent Route(MenuItems menuItem)
    {
        return menuItem switch
        {
            MenuItems.Home => new HomeContentComponent(),
            MenuItems.LatestGeneratedReport => new LatestGeneratedReportComponent(_apiClient),
            MenuItems.Reports => new ReportsOptionsComponent(_apiClient),
            MenuItems.Emails => new EmailsOptionsComponent(_apiClient),
            MenuItems.About => new AboutContentComponent(),
            _ => new HomeContentComponent()
        };
    }
}