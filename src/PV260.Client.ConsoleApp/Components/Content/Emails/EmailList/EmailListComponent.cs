using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content.Common;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using PV260.Common.Models;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Emails.EmailList;

internal class EmailListComponent(IApiClient apiClient) : INavigationComponent
{
    private const string HeaderName = "Email recipients List";

    private readonly IApiClient _apiClient = apiClient;

    private readonly ListPageInformation<EmailRecipientModel> _pageInformation = new();

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

        HandlePageStack(key);
    }

    public IRenderable Render()
    {
        var paginationCursor = new PaginationCursor
        {
            PageSize = PaginationSettings.CalculateRecordsPerPage(),
            LastCreatedAt = _pageInformation.ListPageStack.Model?.CreatedAt,
            LastId = _pageInformation.ListPageStack.Model?.CreatedAt is null ? null : Guid.Empty
        };

        var paginatedEmailsResponse = _apiClient.GetAllEmailsAsync(paginationCursor).Result;

        if (!paginatedEmailsResponse.Items.Any())
        {
            return new EmailOptionPanelBuilder()
                .WithHeader(HeaderName)
                .WithError("There was an error getting email recipients. Please try again", MessageSize.Expanded)
                .Build();
        }

        CalculateEmailListPaging(paginatedEmailsResponse.TotalCount);

        _pageInformation.PageSize = paginatedEmailsResponse.Items.Count;
        _pageInformation.SelectedModel = paginatedEmailsResponse.Items[_pageInformation.SelectedPageIndex];
        _pageInformation.ListPageStack.PushModel(paginatedEmailsResponse.Items.Last());

        return new EmailOptionPanelBuilder()
            .WithHeader(HeaderName)
            .WithList(paginatedEmailsResponse.Items, _pageInformation.SelectedPageIndex)
            .WithMessage("Use <- and -> to navigate between records on page", MessageSize.TableRow)
            .Build();
    }

    public void HandleInput(ConsoleKey key, INavigationService navigationService)
    {
        _pageInformation.SelectedPageIndex = key switch
        {
            ConsoleKey.LeftArrow => (_pageInformation.SelectedPageIndex - 1 + _pageInformation.PageSize) %
                                    _pageInformation.PageSize,
            ConsoleKey.RightArrow => (_pageInformation.SelectedPageIndex + 1) % _pageInformation.PageSize,
            _ => _pageInformation.SelectedPageIndex
        };
    }

    private void CalculateEmailListPaging(int recordCount)
    {
        var paginationSettings = PaginationSettings.CalculatePagination(recordCount);

        NavigationItems = Enumerable.Range(1, paginationSettings.NumberOfPages)
            .Select(i => $"Page {i}")
            .ToArray();
    }

    private void HandlePageStack(ConsoleKey key)
    {
        if (SelectedIndex == 0)
        {
            _pageInformation.ListPageStack.ClearStack();
        }

        if (key != ConsoleKey.UpArrow)
        {
            return;
        }

        _pageInformation.ListPageStack.PopModel();
    }
}