using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Content.Emails.EmailContent;
using PV260.Client.ConsoleApp.Components.Content.Emails.EmailList;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Emails.EmailOptions;

internal class EmailsOptionsComponent(IApiClient apiClient) : INavigationComponent
{
    private const string HeaderName = "Report Options List";

    private readonly EmailOptions[] _emailOptions =
    [
        EmailOptions.ListEmailRecipients,
        EmailOptions.AddEmailRecipient,
        EmailOptions.RemoveEmailRecipient,
        EmailOptions.ClearEmailRecipients
    ];

    public int SelectedIndex { get; private set; }

    public bool IsInSubMenu => true;

    public string[] NavigationItems => _emailOptions.Select(option => option.ToFriendlyString()).ToArray();

    public void Navigate(ConsoleKey key)
    {
        SelectedIndex = key switch
        {
            ConsoleKey.UpArrow => (SelectedIndex - 1 + _emailOptions.Length) % _emailOptions.Length,
            ConsoleKey.DownArrow => (SelectedIndex + 1) % _emailOptions.Length,
            _ => SelectedIndex
        };
    }

    public void HandleInput(ConsoleKey key, INavigationService navigationService)
    {
        if (key != ConsoleKey.Enter)
        {
            return;
        }

        var selectedOption = _emailOptions[SelectedIndex];

        IRenderableComponent emailContentComponent = selectedOption switch
        {
            EmailOptions.ListEmailRecipients => new EmailListComponent(apiClient),
            EmailOptions.AddEmailRecipient => new EmailAddComponent(apiClient, navigationService),
            EmailOptions.RemoveEmailRecipient => new EmailRemoveComponent(apiClient, navigationService),
            EmailOptions.ClearEmailRecipients => new EmailClearComponent(apiClient, navigationService),
            _ => new EmailListComponent(apiClient)
        };

        navigationService.Push(emailContentComponent);
    }
    
    public IRenderable Render()
    {
        var emailOptionPanelBuilder = new EmailOptionPanelBuilder()
            .WithHeader(HeaderName)
            .WithList(_emailOptions, SelectedIndex);

        return emailOptionPanelBuilder.Build();
    }
}