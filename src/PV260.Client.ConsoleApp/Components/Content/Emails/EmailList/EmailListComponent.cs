using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using PV260.Common.Models;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Emails.EmailList;

internal class EmailListComponent(IApiClient apiClient) : IAsyncNavigationComponent
{
    private const string HeaderName = "Email recipients List";

    private readonly ListPageInformation<EmailRecipientModel> _pageInfo = new();
    private PaginatedResponse<EmailRecipientModel>? _emails;

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
        if (_emails == null)
        {
            var cursor = new PaginationCursor
            {
                PageSize = PaginationSettings.CalculateRecordsPerPage(),
                LastCreatedAt = _pageInfo.ListPageStack.Model?.CreatedAt,
                LastId = _pageInfo.ListPageStack.Model?.CreatedAt is null ? null : Guid.Empty
            };

            var response = await apiClient.GetAllEmailsAsync(cursor);
            if (response.IsError)
                return BuildErrorPanel("There was an error getting email recipients. Please try again");

            _emails = response.Value;
        }

        if (!_emails.Items.Any())
            return BuildMessagePanel("There are no email recipients to display.");

        UpdatePaging(_emails.TotalCount);
        _pageInfo.PageSize = _emails.Items.Count;
        _pageInfo.SelectedModel = _emails.Items[_pageInfo.SelectedPageIndex];
        _pageInfo.ListPageStack.PushModel(_emails.Items.Last());

        return new EmailOptionPanelBuilder()
            .WithHeader(HeaderName)
            .WithList(_emails.Items, _pageInfo.SelectedPageIndex)
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
        new EmailOptionPanelBuilder()
            .WithHeader(HeaderName)
            .WithError(message, MessageSize.Expanded)
            .Build();

    private static IRenderable BuildMessagePanel(string message) =>
        new EmailOptionPanelBuilder()
            .WithHeader(HeaderName)
            .WithMessage(message, MessageSize.Expanded)
            .Build();
}