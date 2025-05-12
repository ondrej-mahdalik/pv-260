using PV260.Client.ConsoleApp.Components.Interfaces;
using Spectre.Console.Rendering;

namespace PV260.Client.ConsoleApp.Components.Layout.Interfaces;

internal interface ILayoutBuilder
{
    LayoutBuilder WithHeader();
    LayoutBuilder WithNavigation(string[] menuItems, int selected);
    LayoutBuilder WithContent(IRenderable content);
    LayoutBuilder WithFooter();
    Task<IRenderable> BuildAsync();
}