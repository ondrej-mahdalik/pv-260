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

    private List<EmailRecipientModel>? _emails;

    public int SelectedIndex { get; private set; }
    public string[] NavigationItems { get; private set; } = [];
    public bool IsInSubMenu => true;

    public void Navigate(ConsoleKey key)
    {
        if (_emails == null || _emails.Count == 0)
            return;
        
        switch (key)
        {
            case ConsoleKey.UpArrow:
                SelectedIndex--;
                if (SelectedIndex < 0)
                    SelectedIndex = _emails.Count - 1;
                break;
            case ConsoleKey.DownArrow:
                SelectedIndex = (SelectedIndex + 1) % _emails.Count;
                break;
        }
    }

    public async Task<IRenderable> RenderAsync()
    {
        if (_emails == null)
        {
            var response = await apiClient.GetAllEmailsAsync();
            if (response.IsError)
                return BuildErrorPanel("There was an error getting email recipients. Please try again");

            _emails = response.Value.OrderByDescending(x => x.CreatedAt).ToList();
        }

        if (_emails.Count == 0)
            return BuildMessagePanel("There are no email recipients to display.");

        var amountToShow = PaginationSettings.CalculateRecordsPerPage();

        return new EmailOptionPanelBuilder()
            .WithHeader(HeaderName)
            .WithList(_emails.Take(amountToShow), SelectedIndex)
            .WithMessage("Press 'Delete' to remove the selected email recipient.", MessageSize.TableRow)
            .Build();
    }

    public async Task HandleInputAsync(ConsoleKey key, INavigationService navigationService)
    {
        if (key != ConsoleKey.Delete || _emails == null || SelectedIndex >= _emails.Count)
            return;
        
        var email = _emails[SelectedIndex];
        await apiClient.DeleteEmailAsync(email.EmailAddress);
    }

    public IRenderable Render() => throw new NotSupportedException();

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