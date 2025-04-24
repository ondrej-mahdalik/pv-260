using PV260.Client.ConsoleApp.Components.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Content;

internal class AboutContentComponent : IContentComponent
{
    public bool IsInSubMenu => false;

    public IRenderable Render()
    {
        return new Panel(
                "[bold cyan]About PV260 Report Generator[/]\n\nThis application helps you generate reports easily.")
            .Border(BoxBorder.Rounded)
            .Expand();
    }

    public Task HandleInput(ConsoleKeyInfo key)
    {
        return Task.CompletedTask;
    }
}