using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Interfaces;

internal interface INavbarComponent
{
    IRenderable Render(string[] menuItems, int selected);
}