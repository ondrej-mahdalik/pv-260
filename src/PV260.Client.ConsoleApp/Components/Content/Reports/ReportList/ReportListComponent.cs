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

    private readonly ListPageInformation<ReportListModel> _pageInfo = new();
    private PaginatedResponse<ReportListModel>? _reports;

    public int SelectedIndex { get; private set; }
    public string[] NavigationItems { get; private set; } = [];
    public bool IsInSubMenu => true;

    public void Navigate(ConsoleKey key)
    {
        if (NavigationItems.Length == 0) return;

        switch (key)
        {
            case ConsoleKey.UpArrow:
                SelectedIndex = (SelectedIndex - 1 + NavigationItems.Length) % NavigationItems.Length;
                _pageInfo.SelectedPageIndex = 0;
                break;
            case ConsoleKey.DownArrow:
                SelectedIndex = (SelectedIndex + 1) % NavigationItems.Length;
                _pageInfo.SelectedPageIndex = 0;
                break;
        }

        if (SelectedIndex == 0) _pageInfo.ListPageStack.ClearStack();
        if (key == ConsoleKey.UpArrow) _pageInfo.ListPageStack.PopModel();
    }

    public async Task<IRenderable> RenderAsync()
    {
        if (_reports == null)
        {
            var cursor = new PaginationCursor
            {
                PageSize = PaginationSettings.CalculateRecordsPerPage(),
                LastCreatedAt = _pageInfo.ListPageStack.Model?.CreatedAt,
                LastId = _pageInfo.ListPageStack.Model?.Id
            };

            var response = await apiClient.GetAllReportsAsync(cursor);
            if (response.IsError)
                return BuildErrorPanel("There was an error getting reports. Please try again");

            _reports = response.Value;
        }

        if (!_reports.Items.Any())
            return BuildMessagePanel("There are no reports to display.");

        UpdatePaging(_reports.TotalCount);
        _pageInfo.PageSize = _reports.Items.Count;
        _pageInfo.SelectedModel = _reports.Items[_pageInfo.SelectedPageIndex];

        if (_pageInfo.PageSize == PaginationSettings.CalculateRecordsPerPage())
            _pageInfo.ListPageStack.PushModel(_reports.Items.Last());

        return new ReportOptionPanelBuilder()
            .WithHeader(HeaderName)
            .WithList(_reports.Items, _pageInfo.SelectedPageIndex)
            .WithMessage("Use <- and -> to navigate between records on page", MessageSize.TableRow)
            .Build();
    }

    public Task HandleInputAsync(ConsoleKey key, INavigationService navigationService)
    {
        _pageInfo.SelectedPageIndex = key switch
        {
            ConsoleKey.LeftArrow => (_pageInfo.SelectedPageIndex - 1 + _pageInfo.PageSize) % _pageInfo.PageSize,
            ConsoleKey.RightArrow => (_pageInfo.SelectedPageIndex + 1) % _pageInfo.PageSize,
            _ => _pageInfo.SelectedPageIndex
        };

        if (key != ConsoleKey.Enter || _pageInfo.SelectedModel is null)
            return Task.CompletedTask;
        
        var detailComponent = new ReportDetailComponent(apiClient, _pageInfo.SelectedModel.Id);
        navigationService.Push(detailComponent);

        return Task.CompletedTask;
    }

    public IRenderable Render() => throw new NotSupportedException();

    private void UpdatePaging(int recordCount)
    {
        NavigationItems = Enumerable.Range(1, PaginationSettings.CalculatePagination(recordCount).NumberOfPages)
            .Select(i => $"Page {i}")
            .ToArray();
    }

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