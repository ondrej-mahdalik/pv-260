using PV260.Client.BL;
using PV260.Client.ConsoleApp.Components.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content.Emails;

internal class EmailContentComponent(EmailOptions emailOption) : IContentComponent
{
    public bool IsInSubMenu => false;

    public IRenderable Render()
    {
        return emailOption switch
        {
            EmailOptions.ListEmails => new Table()
                .AddColumn("[green]Email Address[/]")
                .AddColumn("[green]Date Added[/]")
                .Expand(),
            
            EmailOptions.AddEmail => new Panel("TODO Add Email")
                .Border(BoxBorder.Rounded)
                .Expand(),
            _ => new Panel("Unknown option")
                .Border(BoxBorder.Rounded)
                .Expand()
        };
    }

    public event EventHandler? ReloadRequested;

    public void HandleInput(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Backspace)
        {
            AnsiConsole.MarkupLine("[yellow]Returning to the email options...[/]");
        }
    }

    private async Task GetEmails()
    {
        
    }
    
    private async Task AddEmail()
    {
        
    }
}