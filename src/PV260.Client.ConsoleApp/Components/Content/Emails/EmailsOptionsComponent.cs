using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.ConsoleApp.Components.Navigation.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Emails;

internal class EmailsOptionsComponent : INavigationComponent
{
    private readonly EmailOptions[] _emailOptions =
    [
        EmailOptions.SendEmail,
        EmailOptions.DisplayLatestEmail,
        EmailOptions.ListEmails,
        EmailOptions.ListEmailRecipients,
        EmailOptions.AddEmailRecipient,
        EmailOptions.RemoveEmailRecipient,
        EmailOptions.ClearEmailRecipients,
    ];

    private readonly IApiClient _apiClient;

    public EmailsOptionsComponent(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public int SelectedIndex { get; private set; }

    public bool IsInSubMenu => true;

    public string[] GetNavigationItems()
    {
        return _emailOptions.Select(option => option.ToString()).ToArray();
    }

    public void Navigate(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.UpArrow:
                SelectedIndex = (SelectedIndex - 1 + _emailOptions.Length) % _emailOptions.Length;
                break;
            case ConsoleKey.DownArrow:
                SelectedIndex = (SelectedIndex + 1) % _emailOptions.Length;
                break;
        }
    }

    public void HandleInput(ConsoleKeyInfo key, INavigationService navigationService)
    {
        if (key.Key == ConsoleKey.Enter)
        {
            AnsiConsole.Clear();

            var selectedOption = _emailOptions[SelectedIndex];

            // Pass the IApiClient instance to EmailContentComponent
            var emailContentComponent = new EmailContentComponent(selectedOption, _apiClient, navigationService);

            navigationService.Push(emailContentComponent);

            AnsiConsole.MarkupLine($"[green]You have selected to:[/] {selectedOption}");
        }
        else if (key.Key == ConsoleKey.Backspace)
        {
            AnsiConsole.Clear();

            navigationService.Pop();

            AnsiConsole.MarkupLine("[yellow]Returning to previous menu...[/]");
        }
    }

    public IRenderable Render()
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("[bold underline green]Email Options List[/]")
            .Expand();

        foreach (var (email, index) in _emailOptions.Select((email, index) => (email, index)))
        {
            var isSelected = index == SelectedIndex;
            var text = isSelected
                ? $"[black on green]> {email}[/]"
                : $"  {email}";
            table.AddRow(text);
        }

        return new Panel(table)
            .Header("Emails", Justify.Center)
            .Border(BoxBorder.Rounded)
            .Expand();
    }
}
