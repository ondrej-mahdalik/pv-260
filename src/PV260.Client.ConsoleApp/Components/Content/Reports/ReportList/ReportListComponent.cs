using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Client.ConsoleApp.Components.Content.Reports.ReportDetail;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using PV260.Common.Models;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Reports.ReportList;

internal class ReportListComponent(IApiClient apiClient) : INavigationComponent
{
    private const string HeaderName = "Reports List";

    private readonly IApiClient _apiClient = apiClient;

    private readonly PageInformation<ReportListModel> _pageInformation = new();

    public int SelectedIndex { get; private set; }

    public string[] NavigationItems { get; private set; } = [];

    public bool IsInSubMenu => true;

    public void Navigate(ConsoleKey key)
    {
        if (NavigationItems.Length == 0)
        {
            return;
        }

        switch (key)
        {
            case ConsoleKey.UpArrow:
                SelectedIndex = (SelectedIndex - 1 + NavigationItems.Length) % NavigationItems.Length;
                _pageInformation.SelectedPageIndex = 0;
                break;
            case ConsoleKey.DownArrow:
                SelectedIndex = (SelectedIndex + 1) % NavigationItems.Length;
                _pageInformation.SelectedPageIndex = 0;
                break;
        }
    }

    public IRenderable Render()
    {
        var reports = _apiClient.GetAllReportsAsync().Result.ToList();

        if (!reports.Any())
        {
            return new ReportOptionPanelBuilder()
                .WithHeader(HeaderName)
                .WithError("There was an error getting reports. Please try again", MessageSize.Expanded)
                .Build();
        }

        var paginationSettings = CalculateRecordListPaging(reports.Count);

        var paginatedRecords = reports
            .OrderBy(record => record.Name)
            .Skip(SelectedIndex * paginationSettings.MaxPageSize)
            .Take(paginationSettings.MaxPageSize)
            .ToList();

        _pageInformation.PageSize = paginatedRecords.Count;
        _pageInformation.SelectedModel = paginatedRecords[_pageInformation.SelectedPageIndex];

        return new ReportOptionPanelBuilder()
            .WithHeader(HeaderName)
            .WithList(paginatedRecords, _pageInformation.SelectedPageIndex)
            .WithMessage("Use <- and -> to navigate between records on page", MessageSize.TableRow)
            .Build();
    }

    public void HandleInput(ConsoleKey key, INavigationService navigationService)
    {
        switch (key)
        {
            case ConsoleKey.LeftArrow:
                _pageInformation.SelectedPageIndex =
                    (_pageInformation.SelectedPageIndex - 1 + _pageInformation.PageSize) % _pageInformation.PageSize;
                break;
            case ConsoleKey.RightArrow:
                _pageInformation.SelectedPageIndex =
                    (_pageInformation.SelectedPageIndex + 1) % _pageInformation.PageSize;
                break;
            case ConsoleKey.Enter:
                if (_pageInformation.SelectedModel is not null)
                {
                    var detailComponent = new ReportDetailComponent(_apiClient, _pageInformation.SelectedModel.Id);
                    navigationService.Push(detailComponent);
                }

                break;
        }
    }

    private PaginationSettings CalculateRecordListPaging(int recordCount)
    {
        var paginationSettings = PaginationSettings.CalculatePagination(recordCount);

        NavigationItems = Enumerable.Range(1, paginationSettings.NumberOfPages)
            .Select(i => $"Page {i}")
            .ToArray();

        return paginationSettings;
    }
}