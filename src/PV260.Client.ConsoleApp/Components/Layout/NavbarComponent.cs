using PV260.Client.ConsoleApp.Components.Interfaces;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components;

internal class NavbarComponent : INavbarComponent
{
    public IRenderable Render(string[] menuItems, int selected)
    {
        var navTable = new Table().HideHeaders().Border(TableBorder.None);

        navTable.AddColumn(new TableColumn("Navbar"));

        for (var index = 0; index < menuItems.Length; index++)
        {
            var label = index == selected
                ? $"[black on yellow]{menuItems[index]}[/]"
                : $"[grey]{menuItems[index]}[/]";

            navTable.AddRow(new Markup(label));
        }

        return new Panel(navTable)
            .Header("Navigation", Justify.Center)
            .Border(BoxBorder.Rounded)
            .Expand();
    }
}