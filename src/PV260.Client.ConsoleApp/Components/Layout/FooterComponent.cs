using PV260.Client.ConsoleApp.Components.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components;

internal class FooterComponent : IFooterComponent
{
    public IRenderable Render()
    {
        var panel = new Panel(
                "[grey]Up/Down to navigate | Enter to select | Backspace to go back | Esc to exit[/]")
            .Border(BoxBorder.Double)
            .Expand().PadLeft(22);

        return panel;
    }

    public bool IsInSubMenu => false;
}