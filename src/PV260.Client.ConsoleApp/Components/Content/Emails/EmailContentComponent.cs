using PV260.Client.ConsoleApp.Components.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Emails;

internal class EmailContentComponent(EmailOptions emailOption) : IContentComponent
{
    public bool IsInSubMenu => false;

    public IRenderable Render()
    {
        var panel = new Panel($"[bold cyan]You have selected: {emailOption}[/]")
            .Border(BoxBorder.Rounded)
            .Expand();

        return panel;
    }

    public Task HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Backspace)
        {
            AnsiConsole.MarkupLine("[yellow]Returning to the email options...[/]");
        }
        return Task.CompletedTask;
    }
}