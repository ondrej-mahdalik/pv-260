using PV260.Client.ConsoleApp.Components.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content;

internal class HomeContentComponent : IContentComponent
{
    public bool IsInSubMenu => false;

    public IRenderable Render()
    {
        return new Panel(
                "[bold green]Welcome to the PV260 Report Generator![/]\n\n" +
                "[yellow]Select a menu option to get started.[/]\n\n" +
                "Use the arrow keys to navigate and [bold]Enter[/] to select an option.\n\n" +
                "Press [bold red]Escape[/] to exit the application.")
            .Border(BoxBorder.Rounded)
            .Expand();
    }

    public event EventHandler? ReloadRequested;

    public void HandleInput(ConsoleKeyInfo key)
    {
    }
}