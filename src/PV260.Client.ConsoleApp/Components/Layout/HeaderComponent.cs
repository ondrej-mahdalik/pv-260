using PV260.Client.ConsoleApp.Components.Layout.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Layout;

internal class HeaderComponent : IHeaderComponent
{
    public IRenderable Render()
    {
        return new Panel("[bold aqua]PV260 Report Generator[/]")
            .Header("[bold]Header[/]", Justify.Center)
            .Border(BoxBorder.Double)
            .Expand();
    }

    public bool IsInSubMenu => false;
}