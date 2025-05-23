using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Client.ConsoleApp.Components.Content.Reports.ReportDetail;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using PV260.Common.Models;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.ReportList;

internal class ReportListComponent(IApiClient apiClient) : IAsyncNavigationComponent
{
    private const string HeaderName = "Reports List";

    public List<ReportListModel>? Reports;

    public int SelectedIndex { get; private set; }
    public string[] NavigationItems { get; private set; } = [];
    public bool IsInSubMenu => true;

    public void Navigate(ConsoleKey key)
    {
        if (Reports == null || Reports.Count == 0)
            return;
        
        switch (key)
        {
            case ConsoleKey.UpArrow:
                SelectedIndex--;
                if (SelectedIndex < 0)
                    SelectedIndex = Reports.Count - 1;
                break;
            case ConsoleKey.DownArrow:
                SelectedIndex = (SelectedIndex + 1) % Reports.Count;
                break;
        }
    }

    public async Task<IRenderable> RenderAsync()
    {
        if (Reports == null)
        {
            var response = await apiClient.GetAllReportsAsync();
            if (response.IsError)
                return BuildErrorPanel("There was an error getting reports. Please try again");

            Reports = response.Value.OrderByDescending(x => x.CreatedAt).ToList();
        }

        if (Reports.Count == 0)
            return BuildMessagePanel("There are no reports to display.");

        var amountToShow = PaginationSettings.CalculateRecordsPerPage();

        return new ReportOptionPanelBuilder()
            .WithHeader(HeaderName)
            .WithList(Reports.Take(amountToShow), SelectedIndex)
            .WithMessage("Press Enter to open selected report detail.", MessageSize.TableRow)
            .Build();
    }

    public Task HandleInputAsync(ConsoleKey key, INavigationService navigationService)
    {
        if (key != ConsoleKey.Enter || Reports == null || Reports.Count <= SelectedIndex)
            return Task.CompletedTask;
        
        var detailComponent = new ReportDetailComponent(apiClient, Reports[SelectedIndex].Id);
        navigationService.Push(detailComponent);

        return Task.CompletedTask;
    }

    public IRenderable Render() => throw new NotSupportedException();

    private static IRenderable BuildErrorPanel(string message) =>
        new ReportOptionPanelBuilder()
            .WithHeader(HeaderName)
            .WithError(message, MessageSize.TableRow)
            .Build();

    private static IRenderable BuildMessagePanel(string message) =>
        new ReportOptionPanelBuilder()
            .WithHeader(HeaderName)
            .WithMessage(message, MessageSize.TableRow)
            .Build();
}