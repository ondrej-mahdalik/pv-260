using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Client.ConsoleApp.Components.Content.Reports.ReportList;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.ReportOptions;

internal class ReportsOptionsComponent(IApiClient apiClient)
    : IAsyncNavigationComponent
{
    private const string HeaderName = "Report Options List";

    private readonly IApiClient _apiClient = apiClient;

    private readonly ReportOptions[] _reportOptions =
    [
        ReportOptions.GenerateNewReport,
        ReportOptions.ListReports
    ];

    public PageStatus PageStatus { get; private set; } = new();

    public int SelectedIndex { get; private set; }

    public bool IsInSubMenu => true;

    public string[] NavigationItems => _reportOptions.Select(option => option.ToFriendlyString()).ToArray();

    public void Navigate(ConsoleKey key)
    {
        if (key is not (ConsoleKey.UpArrow or ConsoleKey.DownArrow))
        {
            return;
        }

        PageStatus = new PageStatus();

        var delta = key == ConsoleKey.UpArrow ? -1 : 1;

        SelectedIndex = (SelectedIndex + delta + _reportOptions.Length) % _reportOptions.Length;
    }

    public Task<IRenderable> RenderAsync()
    {
        var reportOptionPanelBuilder = new ReportOptionPanelBuilder()
            .WithHeader(HeaderName)
            .WithList(_reportOptions, SelectedIndex);

        switch (PageStatus)
        {
            case { IsSuccess: true }:
                reportOptionPanelBuilder.WithSuccess(PageStatus.StatusMessage, MessageSize.TableRow);
                break;

            case { IsSuccess: false }:
                reportOptionPanelBuilder.WithError(PageStatus.StatusMessage, MessageSize.TableRow);
                break;
        }

        return Task.FromResult(reportOptionPanelBuilder.Build());
    }

    public async Task HandleInputAsync(ConsoleKey key, INavigationService navigationService)
    {
        if (key != ConsoleKey.Enter)
        {
            return;
        }

        PageStatus = new PageStatus();

        var selectedOption = _reportOptions[SelectedIndex];

        switch (selectedOption)
        {
            case ReportOptions.GenerateNewReport:
                await GenerateNewReportAsync();
                break;

            case ReportOptions.ListReports:
                navigationService.Push(new ReportListComponent(_apiClient));
                break;
        }
    }
    
    public IRenderable Render()
        => throw new NotSupportedException();

    private async Task GenerateNewReportAsync()
    {
        try
        {
            await _apiClient.GenerateNewReportAsync();

            PageStatus = new PageStatus
            {
                IsSuccess = true,
                StatusMessage = "New report generated successfully!"
            };
        }
        catch (HttpRequestException)
        {
            PageStatus = new PageStatus
            {
                IsSuccess = false,
                StatusMessage = "Unable to connect to the server. Please make sure the API server is running!"
            };
        }
        catch (Exception)
        {
            PageStatus = new PageStatus
            {
                IsSuccess = false,
                StatusMessage = "Failed to generate report"
            };
        }
    }
}